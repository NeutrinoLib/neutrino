using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neutrino.Api.Consensus.Events;
using Neutrino.Api.Consensus.Responses;
using Neutrino.Api.Consensus.States;
using Neutrino.Core.Services.Parameters;
using Neutrino.Entities;

namespace Neutrino.Api.Consensus
{
    public class ConsensusContext : IConsensusContext
    {
        private int _electionTimeout;
        private int _currentTerm = 1;
        private State _state;
        private readonly ApplicationParameters _applicationParameters;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IList<NodeState> _nodeStates;
        private readonly NodeVote _nodeVote;

        public ConsensusContext(
            IOptions<ApplicationParameters> applicationParameters, 
            IApplicationLifetime applicationLifetime)
        {
            _applicationParameters = applicationParameters.Value;
            _applicationLifetime = applicationLifetime;

            _applicationLifetime.ApplicationStopping.Register(DisposeResources);

            _nodeVote = new NodeVote();
            _nodeStates = new List<NodeState>();
            foreach(var node in _applicationParameters.Nodes)
            {
                _nodeStates.Add(new NodeState { Node = node, VoteGranted = false});
            }
        }

        public void Run()
        {
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
                _applicationParameters.MinElectionTimeout, 
                _applicationParameters.MaxElectionTimeout);

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
                return _applicationParameters.CurrentNode;
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
            get { return _applicationParameters.HeartbeatTimeout; }
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