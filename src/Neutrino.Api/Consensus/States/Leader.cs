using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neutrino.Api.Consensus.Events;
using Neutrino.Entities;
using Newtonsoft.Json;

namespace Neutrino.Api.Consensus.States
{
    public class Leader : State
    {
        private int _heartbeatTimeout = 400;
        private int _lastSentHeartbeat = 0;
        private readonly HttpClient _httpClient;
        private readonly IConsensusContext _consensusContext;
        private CancellationTokenSource _sendHeartbeatTokenSource;

        public Leader(IConsensusContext consensusContext)
        {
            _consensusContext = consensusContext;
            _httpClient = new HttpClient();

            _consensusContext.LeaderNode = _consensusContext.CurrentNode;
        }

        public override void Proceed()
        {
            StartSendingHeartbeats();
        }

        public override string ToString()
        {
            return $"Leader";
        }

        public override void Dispose()
        {
            Dispose(true);
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
            foreach(var node in _consensusContext.Nodes)
            {
                var task = SendHeartbeat(node);
                tasks.Add(task);
            }
        }

        private Task<HttpResponseMessage> SendHeartbeat(Node node)
        {
            var url = Path.Combine(node.Address, "api/consensus/heartbeat");
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var hearbeatEvent = new HeartbeatEvent(_consensusContext.CurrentTerm, _consensusContext.CurrentNode);
            var jsonContent = JsonConvert.SerializeObject(hearbeatEvent);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;

            var task = _httpClient.SendAsync(request);
            return task;
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _sendHeartbeatTokenSource.Cancel();
                    _httpClient.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}