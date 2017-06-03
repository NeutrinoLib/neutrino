using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neutrino.Api.Consensus.Events;
using Neutrino.Api.Consensus.Responses;
using Neutrino.Api.Consensus.States;
using Neutrino.Consensus.Options;
using Neutrino.Entities;

namespace Neutrino.Api.Consensus
{
    public class ConsensusContext : IConsensusContext
    {
        private int _electionTimeout;
        private int _currentTerm = 1;
        private State _state;
        private ConsensusOptions _consensusOptions;
        private IList<NodeState> _nodeStates;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly NodeVote _nodeVote;

        public ConsensusContext(IApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
            _applicationLifetime.ApplicationStopping.Register(DisposeResources);
            _nodeVote = new NodeVote();
        }

        public void Run(ConsensusOptions consensusOptions)
        {
            _consensusOptions = consensusOptions;

            _nodeStates = new List<NodeState>();
            foreach(var node in _consensusOptions.Nodes)
            {
                _nodeStates.Add(new NodeState { Node = node, VoteGranted = false });
            }

            RandomElectionTimeout();
            State = new Follower(this);
        }

        public IResponse TriggerEvent(IEvent triggeredEvent)
        {
            return State.TriggerEvent(triggeredEvent);
        }

        private void RandomElectionTimeout()
        {
            var random = new Random();
            _electionTimeout = random.Next(
                _consensusOptions.MinElectionTimeout, 
                _consensusOptions.MaxElectionTimeout);

            Console.WriteLine($"Election timeout was calculated: {_electionTimeout}");
        }

        public Node LeaderNode { get; set; }

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
                _state = value;
                Console.WriteLine($"Node is now in '{_state.GetType().Name}' state.");

                _state.Proceed();
            }
        }

        public Node CurrentNode
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