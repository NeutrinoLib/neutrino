using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neutrino.Api.Consensus.Events;
using Neutrino.Api.Consensus.Responses;
using Neutrino.Entities;
using Newtonsoft.Json;

namespace Neutrino.Api.Consensus.States
{
    public class Candidate : State
    {
        private int _lastVoting = int.MaxValue;
        private readonly HttpClient _httpClient;
        private readonly IConsensusContext _consensusContext;
        private CancellationTokenSource _votingTokenSource;

        public Candidate(IConsensusContext consensusContext)
        {
            _consensusContext = consensusContext;
            _httpClient = new HttpClient();

            ClearVoteGranted();
        }

        public override void Proceed()
        {
            OpenVoting();
        }

        public override IResponse TriggerEvent(IEvent triggeredEvent)
        {
            var leaderRequestEvent = triggeredEvent as LeaderRequestEvent;
            if(leaderRequestEvent != null)
            {
                Console.WriteLine($"Retreived 'LeaderRequestEvent' event (currentTerm: {leaderRequestEvent.CurrentTerm}, node: {leaderRequestEvent.Node.Id}).");

                bool voteGranted = OtherNodeCanBeLeader(leaderRequestEvent);
                if(voteGranted)
                {
                    _consensusContext.CurrentTerm = leaderRequestEvent.CurrentTerm;
                    _consensusContext.NodeVote.LeaderNode = leaderRequestEvent.Node;
                    _consensusContext.NodeVote.VoteTerm = leaderRequestEvent.CurrentTerm;

                    Console.WriteLine($"Voting for node ({leaderRequestEvent.Node.Id}): GRANTED.");

                    StopVoting();
                    _consensusContext.State = new Follower(_consensusContext);
                }
                else
                {
                    Console.WriteLine($"Voting for node ({leaderRequestEvent.Node.Id}): NOT GRANTED.");
                }
                
                return new VoteResponse(voteGranted, _consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            }

            return new EmptyResponse();
        }

        public override void DisposeCore()
        {
            _httpClient.Dispose();
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

                    var nodeVote = JsonConvert.DeserializeObject<VoteResponse>(responseContent);
                    _consensusContext.CurrentTerm = nodeVote.CurrentTerm;

                    var nodeState = _consensusContext.NodeStates.FirstOrDefault(x => x.Node.Id == nodeVote.Node.Id);
                    if(nodeState != null)
                    {
                        nodeState.VoteGranted = nodeVote.VoteGranted;
                    }

                    Console.WriteLine($"Vote from node ({nodeVote.Node.Id}): {nodeVote.VoteGranted} (term: {_consensusContext.CurrentTerm}).");
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

        private Task<HttpResponseMessage> SendLeaderRequestVote(Node node)
        {
            var url = Path.Combine(node.Address, "api/raft/leader");
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var leaderRequest = new LeaderRequestEvent(_consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            var jsonContent = JsonConvert.SerializeObject(leaderRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;

            var task = _httpClient.SendAsync(request);
            return task;
        }

        private bool CurrentNodeCanBeLeader()
        {
            var amountOfGrantedVotes = _consensusContext.NodeStates.Count(x => x.VoteGranted) + 1;
            var allNodes = _consensusContext.NodeStates.Count + 1;

            return (amountOfGrantedVotes * 2) >= allNodes;
        }

        private bool OtherNodeCanBeLeader(LeaderRequestEvent leaderRequestEvent)
        {
            return leaderRequestEvent.CurrentTerm > _consensusContext.CurrentTerm 
                && leaderRequestEvent.CurrentTerm > _consensusContext.NodeVote.VoteTerm;
        }
    }
}