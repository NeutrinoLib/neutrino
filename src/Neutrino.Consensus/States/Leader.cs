using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neutrino.Consensus.Entities;
using Neutrino.Consensus.Events;
using Neutrino.Consensus.Responses;
using Newtonsoft.Json;

namespace Neutrino.Consensus.States
{
    public class Leader : State
    {
        private int _lastSentHeartbeat = int.MaxValue;
        private readonly IConsensusContext _consensusContext;
        private readonly ILogger<IConsensusContext> _logger;
        private CancellationTokenSource _sendHeartbeatTokenSource;

        public Leader(IConsensusContext consensusContext, ILogger<IConsensusContext> logger)
        {
            _consensusContext = consensusContext;
            _logger = logger;

            _consensusContext.NodeVote.LeaderNode = _consensusContext.CurrentNode;
            _consensusContext.NodeVote.VoteTerm = 0;
        }

        public override void Proceed()
        {
            StartSendingHeartbeats();
        }

        public override IResponse TriggerEvent(IEvent triggeredEvent)
        {
            var requestVoteEvent = triggeredEvent as RequestVoteEvent;
            if(requestVoteEvent != null)
            {
                return new RequestVoteResponse(false, _consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            }

            return new EmptyResponse();
        }

        public override void DisposeCore()
        {
            _sendHeartbeatTokenSource.Cancel();
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

        private Task<HttpResponseMessage> SendHeartbeat(NodeInfo node)
        {
            var url = Path.Combine(node.Address, "api/raft/append-entries");
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue(
                _consensusContext.ConsensusOptions.AuthenticationScheme, _consensusContext.ConsensusOptions.AuthenticationParameter);

            var appendEntriesEvent = new AppendEntriesEvent(_consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            var jsonContent = JsonConvert.SerializeObject(appendEntriesEvent);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;

            var task = _consensusContext.HttpClient.SendAsync(request);
            return task;
        }
    }
}