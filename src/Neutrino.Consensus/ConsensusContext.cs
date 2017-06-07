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

namespace Neutrino.Consensus
{
    public class ConsensusContext : IConsensusContext
    {
        private int _electionTimeout;
        private int _currentTerm = 1;
        private State _state;
        private ConsensusOptions _consensusOptions;
        private IList<NodeState> _nodeStates;
        private readonly IStateObservable _stateObservable;
        private readonly ILogReplicable _logReplicable;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly NodeVote _nodeVote;

        public ConsensusContext(IApplicationLifetime applicationLifetime, IStateObservable stateObservable, ILogReplicable logReplicable)
        {
            _applicationLifetime = applicationLifetime;
            _stateObservable = stateObservable;
            _logReplicable = logReplicable;

            _applicationLifetime.ApplicationStopping.Register(DisposeResources);
            _nodeVote = new NodeVote();
        }

        public void Run(ConsensusOptions consensusOptions)
        {
            _consensusOptions = consensusOptions;

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