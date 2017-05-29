using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neutrino.Core.Services.Parameters;
using Neutrino.Entities;
using Newtonsoft.Json;

namespace Neutrino.Api.Consensus
{
    public class LeaderElectionService : ILeaderElectionService
    {
        private int _electionTimeout;
        private int _heartbeatTimeout = 5000;
        private int _lastRertievedHeartbeat = 0;
        private int _lastSentHeartbeat = 0;
        private int _term = 0;
        private NodeState _nodeState;
        private Node _currentLeader;
        private HttpClient _httpClient;
        private ApplicationParameters _applicationParameters;
        private ILogger<LeaderElectionService> _logger;
        private CancellationTokenSource _checkHeartbeatTokenSource;
        private CancellationTokenSource _sendHeartbeatTokenSource;

        public LeaderElectionService(
            HttpClient httpClient, 
            IOptions<ApplicationParameters> applicationParameters,
            ILogger<LeaderElectionService> logger)
        {
            _httpClient = httpClient;
            _applicationParameters = applicationParameters.Value;
            _logger = logger;

            if (_applicationParameters.CurrentNode == null)
            {
                throw new ArgumentNullException("Consensus requires current node configuration.");
            }
        }

        public void Run()
        {
            SetNodeState(NodeState.Follower);
            RandomElectionTimeout();
            StartCheckingHeartbeats();
        }

        public void ReceiveHearbeat(Node node)
        {
            _currentLeader = node;
            _lastRertievedHeartbeat = 0;
        }

        public bool ReceiveLeaderRequest(Node node)
        {
            if(OtherNodeCanBeLeader(node))
            {
                return true;
            }

            return false;
        }

        private void SetNodeState(NodeState nodeState)
        {
            _nodeState = nodeState;
            _logger.LogInformation($"Node is now in '{nodeState}' state.");
        }

        private void RandomElectionTimeout()
        {
            var random = new Random();
            _electionTimeout = random.Next(6000, 12000);
            _logger.LogInformation($"Election timeout in node is: {_electionTimeout}ms.");
        }

        private void StartCheckingHeartbeats()
        {
            _checkHeartbeatTokenSource = new CancellationTokenSource();
            Task.Run(() => CheckHeartbeat(_checkHeartbeatTokenSource.Token), _checkHeartbeatTokenSource.Token);
        }

        private void StopCheckingHeartbeat()
        {
            _checkHeartbeatTokenSource.Cancel();
        }

        private async Task CheckHeartbeat(CancellationToken token)
        {
            while(!token.IsCancellationRequested) 
            {
                if(_lastRertievedHeartbeat > _electionTimeout)
                {
                    SetNodeState(NodeState.Candidate);
                    await SendLeaderRequestVotes();
                }

                Thread.Sleep(50);
                _lastRertievedHeartbeat += 50;
            }
        }

        private void StartSendingHeartbeats()
        {
            _sendHeartbeatTokenSource = new CancellationTokenSource();
            Task.Run(() => SendingHeartbeats(_sendHeartbeatTokenSource.Token), _sendHeartbeatTokenSource.Token);
        }

        private void StopSendingHeartbeat()
        {
            _sendHeartbeatTokenSource.Cancel();
        }

        private void SendingHeartbeats(CancellationToken token)
        {
            while(!token.IsCancellationRequested) 
            {
                if(_lastSentHeartbeat > _heartbeatTimeout)
                {
                    SendHeartbeats();
                    _lastSentHeartbeat = 0;
                }

                Thread.Sleep(50);
                _lastSentHeartbeat += 50;
            }
        }

        private void SendHeartbeats()
        {
            var tasks = new List<Task<HttpResponseMessage>>();
            foreach(var node in _applicationParameters.Nodes)
            {
                var task = SendHeartbeat(node);
                tasks.Add(task);
            }
        }

        private Task<HttpResponseMessage> SendHeartbeat(Node node)
        {
            var url = Path.Combine(node.Address, "api/consensus/heartbeat");
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var jsonContent = JsonConvert.SerializeObject(_applicationParameters.CurrentNode);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;

            _logger.LogInformation("Sending heartbeat to other nodes...");
            var task = _httpClient.SendAsync(request);
            return task;
        }

        private async Task SendLeaderRequestVotes()
        {
            if(_applicationParameters.Nodes == null || _applicationParameters.Nodes.Length == 0)
            {
                SetNodeState(NodeState.Leader);
                StopCheckingHeartbeat();
                return;
            }

            var tasks = new List<Task<HttpResponseMessage>>();
            try
            {
                foreach(var node in _applicationParameters.Nodes)
                {
                    var task = SendLeaderRequestVote(node);
                    tasks.Add(task);
                }

                _logger.LogInformation($"Waiting for response...");
                await Task.WhenAll(tasks.ToArray());
            }
            catch(Exception)
            {
                _logger.LogInformation($"Node is in failing state.");
            }
            
            int votesNumber = 0;
            foreach(var task in tasks)
            {
                if(task.Status == TaskStatus.RanToCompletion && task.Result.IsSuccessStatusCode)
                {
                    var responseContent = await task.Result.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Response from node: {responseContent}");
                    var nodeVote = JsonConvert.DeserializeObject<NodeVote>(responseContent);
                    if(nodeVote.VoteValue)
                    {
                        votesNumber++;
                    }
                }
                else
                {
                    votesNumber++;
                }
            }

            _logger.LogInformation($"Amount of votes: {votesNumber}");
            if(CurrentNodeCanBeLeader(votesNumber, _applicationParameters.Nodes))
            {
                SetNodeState(NodeState.Leader);
                StopCheckingHeartbeat();
                StartSendingHeartbeats();
            }
        }

        private Task<HttpResponseMessage> SendLeaderRequestVote(Node node)
        {
            var url = Path.Combine(node.Address, "api/consensus/leader");
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var jsonContent = JsonConvert.SerializeObject(_applicationParameters.CurrentNode);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;

            _logger.LogInformation("Sending leader request to other nodes...");
            var task = _httpClient.SendAsync(request);
            return task;
        }

        private bool CurrentNodeCanBeLeader(int votesNumber, Node[] nodes)
        {
            return (votesNumber * 2) >= nodes.Length;
        }

        private bool OtherNodeCanBeLeader(Node node)
        {
            return _nodeState == NodeState.Follower;
        }
    }
}