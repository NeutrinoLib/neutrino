using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Neutrino.Api.Consensus.Events;
using Neutrino.Api.Consensus.Responses;
using Neutrino.Entities;
using Newtonsoft.Json;

namespace Neutrino.Api.Consensus.States
{
    public class Candidate : State
    {
        private bool disposedValue = false;
        private readonly HttpClient _httpClient;
        private readonly IConsensusContext _consensusContext;

        public Candidate(IConsensusContext consensusContext)
        {
            _consensusContext = consensusContext;
            _httpClient = new HttpClient();
        }

        public override void Proceed()
        {
            OpenVoting();
        }

        public override string ToString()
        {
            return $"Candidate";
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        private void OpenVoting()
        {
            _consensusContext.CurrentTerm++;

            if (_consensusContext.Nodes == null || _consensusContext.Nodes.Length == 0)
            {
                _consensusContext.State = new Leader(_consensusContext);
                return;
            }

            List<Task<HttpResponseMessage>> tasks = SendLeaderRequestVotes();
            int votesNumber = CalculateVotes(tasks);

            if (CurrentNodeCanBeLeader(votesNumber, _consensusContext.Nodes))
            {
                _consensusContext.State = new Leader(_consensusContext);
            }
            else
            {
                _consensusContext.State = new Follower(_consensusContext);
            }
        }

        private int CalculateVotes(List<Task<HttpResponseMessage>> tasks)
        {
            int votesNumber = 0;
            foreach (var task in tasks)
            {
                if (task.Status == TaskStatus.RanToCompletion && task.Result.IsSuccessStatusCode)
                {
                    var responseContent = task.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var nodeVote = JsonConvert.DeserializeObject<VoteResponse>(responseContent);
                    if (nodeVote.VoteValue)
                    {
                        votesNumber++;
                    }
                }
                else
                {
                    votesNumber++;
                }
            }

            return votesNumber;
        }

        private List<Task<HttpResponseMessage>> SendLeaderRequestVotes()
        {
            var tasks = new List<Task<HttpResponseMessage>>();
            try
            {
                foreach (var node in _consensusContext.Nodes)
                {
                    var task = SendLeaderRequestVote(node);
                    tasks.Add(task);
                }

                Task.WhenAll(tasks.ToArray()).GetAwaiter();
            }
            catch (Exception)
            {
            }

            return tasks;
        }

        private Task<HttpResponseMessage> SendLeaderRequestVote(Node node)
        {
            var url = Path.Combine(node.Address, "api/consensus/leader");
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var leaderRequest = new LeaderRequestEvent(_consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            var jsonContent = JsonConvert.SerializeObject(leaderRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;

            var task = _httpClient.SendAsync(request);
            return task;
        }

        private bool CurrentNodeCanBeLeader(int votesNumber, Node[] nodes)
        {
            return (votesNumber * 2) >= nodes.Length;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}