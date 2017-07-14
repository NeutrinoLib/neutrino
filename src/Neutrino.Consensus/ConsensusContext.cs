using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neutrino.Consensus.Entities;
using Neutrino.Consensus.Events;
using Neutrino.Consensus.Responses;
using Neutrino.Consensus.States;
using Neutrino.Consensus.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;

namespace Neutrino.Consensus
{
    public class ConsensusContext : IConsensusContext
    {
        private int _electionTimeout;
        private int _currentTerm = 1;
        private State _state;
        private ConsensusOptions _consensusOptions;
        private IList<NodeState> _nodeStates;
        private IStateObservable _stateObservable;
        private ILogReplicable _logReplicable;
        private HttpClient _httpClient;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly NodeVote _nodeVote;

        public ConsensusContext(IApplicationLifetime applicationLifetime, HttpClient httpClient)
        {
            _applicationLifetime = applicationLifetime;
            _httpClient = httpClient;

            _applicationLifetime.ApplicationStopping.Register(DisposeResources);
            _nodeVote = new NodeVote();
        }

        public void Run(
            ConsensusOptions consensusOptions, 
            IStateObservable stateObservable, 
            ILogReplicable logReplicable)
        {
            _consensusOptions = consensusOptions;
            _stateObservable = stateObservable;
            _logReplicable = logReplicable;

            _nodeStates = new List<NodeState>();
            if(_consensusOptions.Nodes != null)
            {
                foreach(var node in _consensusOptions.Nodes)
                {
                    _nodeStates.Add(new NodeState { Node = node, VoteGranted = false });
                }
            }

            RandomElectionTimeout();
            State = new Follower(this);
        }

        public void EnsureLogConsistency()
        {
            Console.WriteLine("Ensure log is consistency process started...");

            _logReplicable.OnClearLog();

            var url = Path.Combine(_nodeVote.LeaderNode.Address, "api/raft/full-log");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue(
                ConsensusOptions.AuthenticationScheme, ConsensusOptions.AuthenticationParameter);

            var httpResponseMessage = _httpClient.SendAsync(request).GetAwaiter().GetResult();

            if(httpResponseMessage.IsSuccessStatusCode)
            {
                var content = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var appendEntriesEvent = JsonConvert.DeserializeObject<AppendEntriesEvent>(content);
                _logReplicable.OnLogReplication(appendEntriesEvent);
            }

            Console.WriteLine("Process of log consistency finished");
        }

        private void RandomElectionTimeout()
        {
            var random = new Random();
            _electionTimeout = random.Next(
                _consensusOptions.MinElectionTimeout, 
                _consensusOptions.MaxElectionTimeout);

            Console.WriteLine($"Election timeout was calculated: {_electionTimeout}");
        }

        public NodeInfo LeaderNode { get; set; }

        public int CurrentTerm 
        { 
            get 
            {
                return _currentTerm;
            }
            set
            {
                if(value > _currentTerm)
                {
                    _currentTerm = value;
                }
            }
        }

        public State State 
        {
            get { return _state; }
            set
            {
                var oldState = _state;
                _stateObservable.OnStateChanging(oldState, value);

                _state = value;
                Console.WriteLine($"Node is now in '{_state.GetType().Name}' state.");
                _state.Proceed();

                _stateObservable.OnStateChanged(oldState, value);
            }
        }

        public NodeInfo CurrentNode
        {
            get
            {
                return _consensusOptions.CurrentNode;
            }
        }

        public IList<NodeState> NodeStates
        {
            get 
            {
                return _nodeStates;
            }
        }

        public int HeartbeatTimeout 
        {
            get { return _consensusOptions.HeartbeatTimeout; }
        }

        public int ElectionTimeout
        {   
            get { return _electionTimeout; }
        }

        public NodeVote NodeVote 
        { 
            get { return _nodeVote; } 
        }

        public ILogReplicable LogReplicable 
        { 
            get { return _logReplicable; }
        }

        public IStateObservable StateObservable 
        { 
            get { return _stateObservable; }
        }

        public HttpClient HttpClient
        {
            get { return _httpClient; }
        }

        public ConsensusOptions ConsensusOptions
        {
            get { return _consensusOptions; }
        } 

        public bool IsLeader()
        {
            return State is Leader;
        }

        protected void DisposeResources()
        {
            Console.WriteLine("Canceling consensus events...");
            if(_state != null)
            {
                _state.Dispose();
                _state = null;
            }
        }
    }
}