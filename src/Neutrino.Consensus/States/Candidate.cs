using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neutrino.Consensus.Entities;
using Neutrino.Consensus.Events;
using Neutrino.Consensus.Responses;
using Newtonsoft.Json;

namespace Neutrino.Consensus.States
{
    public class Candidate : State
    {
        private int _lastVoting = int.MaxValue;
        private readonly IConsensusContext _consensusContext;
        private CancellationTokenSource _votingTokenSource;

        public Candidate(IConsensusContext consensusContext)
        {
            _consensusContext = consensusContext;

            ClearVoteGranted();
        }

        public override void Proceed()
        {
            OpenVoting();
        }

        public override IResponse TriggerEvent(IEvent triggeredEvent)
        {
            var requestVoteEvent = triggeredEvent as RequestVoteEvent;
            if(requestVoteEvent != null)
            {
                Console.WriteLine($"Retreived 'LeaderRequestEvent' event (currentTerm: {requestVoteEvent.Term}, node: {requestVoteEvent.Node.Id}).");

                bool voteGranted = OtherNodeCanBeLeader(requestVoteEvent);
                if(voteGranted)
                {
                    _consensusContext.CurrentTerm = requestVoteEvent.Term;
                    _consensusContext.NodeVote.LeaderNode = requestVoteEvent.Node;
                    _consensusContext.NodeVote.VoteTerm = requestVoteEvent.Term;

                    Console.WriteLine($"Voting for node ({requestVoteEvent.Node.Id}): GRANTED.");

                    StopVoting();
                    _consensusContext.State = new Follower(_consensusContext);
                }
                else
                {
                    Console.WriteLine($"Voting for node ({requestVoteEvent.Node.Id}): NOT GRANTED.");
                }
                
                return new RequestVoteResponse(voteGranted, _consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            }

            return new EmptyResponse();
        }

        public override void DisposeCore()
        {
        }

        private void OpenVoting()
        {
            if (_consensusContext.NodeStates == null || _consensusContext.NodeStates.Count == 0)
            {
                _consensusContext.State = new Leader(_consensusContext);
                return;
            }

            _votingTokenSource = new CancellationTokenSource();
            Task.Run(() => StartVoting(_votingTokenSource.Token), _votingTokenSource.Token);
        }

        private void StopVoting()
        {
            _votingTokenSource.Cancel();
        }

        private void StartVoting(CancellationToken token)
        {
            while(!token.IsCancellationRequested) 
            {
                if(_lastVoting >= _consensusContext.ElectionTimeout)
                {
                    _consensusContext.CurrentTerm++;
                    var tasks = SendLeaderRequestVotes();
                    
                    if(!CollectVotes(tasks))
                    {
                        Console.WriteLine($"All other nodes are disabled. Current node can be a leader.");
                        _consensusContext.State = new Leader(_consensusContext);
                        break;
                    }

                    if (CurrentNodeCanBeLeader())
                    {
                        _consensusContext.State = new Leader(_consensusContext);
                        break;
                    }

                    _lastVoting = 0;
                }

                Thread.Sleep(50);
                _lastVoting += 50;
            }
        }

        private bool CollectVotes(List<Task<HttpResponseMessage>> tasks)
        {
            var amountOfFailed = 0;
            foreach (var task in tasks)
            {
                if (task.Status == TaskStatus.RanToCompletion && task.Result.IsSuccessStatusCode)
                {
                    var responseContent = task.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    var requestVoteResponse = JsonConvert.DeserializeObject<RequestVoteResponse>(responseContent);
                    _consensusContext.CurrentTerm = requestVoteResponse.CurrentTerm;

                    var nodeState = _consensusContext.NodeStates.FirstOrDefault(x => x.Node.Id == requestVoteResponse.Node.Id);
                    if(nodeState != null)
                    {
                        nodeState.VoteGranted = requestVoteResponse.VoteGranted;
                    }

                    Console.WriteLine($"Vote from node ({requestVoteResponse.Node.Id}): {requestVoteResponse.VoteGranted} (term: {_consensusContext.CurrentTerm}).");
                }
                else
                {
                    Console.WriteLine($"Vote failed with task status: {task.Status} (term: {_consensusContext.CurrentTerm}).");
                    amountOfFailed++;
                }
            }

            return amountOfFailed < tasks.Count;
        }

        private void ClearVoteGranted()
        {
            foreach (var nodeState in _consensusContext.NodeStates)
            {
                nodeState.VoteGranted = false;
            }
        }

        private List<Task<HttpResponseMessage>> SendLeaderRequestVotes()
        {
            var tasks = new List<Task<HttpResponseMessage>>();
            try
            {
                foreach (var nodeState in _consensusContext.NodeStates.Where(x => x.VoteGranted == false))
                {
                    var task = SendLeaderRequestVote(nodeState.Node);
                    tasks.Add(task);
                }

                Task.WhenAll(tasks.ToArray()).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
            }

            return tasks;
        }

        private Task<HttpResponseMessage> SendLeaderRequestVote(NodeInfo node)
        {
            var url = Path.Combine(node.Address, "api/raft/request-vote");
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue(
                _consensusContext.ConsensusOptions.AuthenticationScheme, _consensusContext.ConsensusOptions.AuthenticationParameter);

            var requestVoteEvent = new RequestVoteEvent(_consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            var jsonContent = JsonConvert.SerializeObject(requestVoteEvent);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;

            var task = _consensusContext.HttpClient.SendAsync(request);
            return task;
        }

        private bool CurrentNodeCanBeLeader()
        {
            var amountOfGrantedVotes = _consensusContext.NodeStates.Count(x => x.VoteGranted) + 1;
            var allNodes = _consensusContext.NodeStates.Count + 1;

            return (amountOfGrantedVotes * 2) >= allNodes;
        }

        private bool OtherNodeCanBeLeader(RequestVoteEvent requestVoteEvent)
        {
            return requestVoteEvent.Term > _consensusContext.CurrentTerm 
                && requestVoteEvent.Term > _consensusContext.NodeVote.VoteTerm;
        }
    }
}