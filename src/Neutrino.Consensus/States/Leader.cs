using System;
using System.Collections.Generic;
using System.IO;
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
    public class Leader : State
    {
        private int _lastSentHeartbeat = int.MaxValue;
        private readonly HttpClient _httpClient;
        private readonly IConsensusContext _consensusContext;
        private CancellationTokenSource _sendHeartbeatTokenSource;

        public Leader(IConsensusContext consensusContext)
        {
            _consensusContext = consensusContext;
            _httpClient = new HttpClient();

            _consensusContext.NodeVote.LeaderNode = _consensusContext.CurrentNode;
            _consensusContext.NodeVote.VoteTerm = 0;
        }

        public override void Proceed()
        {
            StartSendingHeartbeats();
        }

        public override IResponse TriggerEvent(IEvent triggeredEvent)
        {
            var leaderRequestEvent = triggeredEvent as LeaderRequestEvent;
            if(leaderRequestEvent != null)
            {
                return new VoteResponse(false, _consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            }

            return new EmptyResponse();
        }

        public override void DisposeCore()
        {
            _sendHeartbeatTokenSource.Cancel();
            _httpClient.Dispose();
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
                if(_lastSentHeartbeat >= _consensusContext.HeartbeatTimeout)
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
            foreach(var nodeState in _consensusContext.NodeStates)
            {
                var task = SendHeartbeat(nodeState.Node);
                tasks.Add(task);
            }
        }

        private Task<HttpResponseMessage> SendHeartbeat(Node node)
        {
            var url = Path.Combine(node.Address, "api/raft/heartbeat");
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var hearbeatEvent = new HeartbeatEvent(_consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            var jsonContent = JsonConvert.SerializeObject(hearbeatEvent);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;

            var task = _httpClient.SendAsync(request);
            return task;
        }
    }
}