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
        private readonly ILogger<ConsensusContext> _logger;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IList<NodeState> _nodeStates;


        public ConsensusContext(
            IOptions<ApplicationParameters> applicationParameters, 
            ILogger<ConsensusContext> logger,
            IApplicationLifetime applicationLifetime)
        {
            _applicationParameters = applicationParameters.Value;
            _logger = logger;
            _applicationLifetime = applicationLifetime;

            _applicationLifetime.ApplicationStopping.Register(DisposeResources);

            _nodeStates = new List<NodeState>();
            foreach(var node in _applicationParameters.Nodes)
            {
                _nodeStates.Add(new NodeState { Node = node, VoteGranted = false});
            }

            RandomElectionTimeout();
        }

        public void Run()
        {
            State = new Follower(this);
        }

        public IResponse TriggerEvent(IEvent triggeredEvent)
        {
            //_logger.LogInformation($"Receive trigger event: '{triggeredEvent.GetType().Name}'.");
            Console.WriteLine($"Receive trigger event: '{triggeredEvent.GetType().Name}'.");
            return State.TriggerEvent(triggeredEvent);
        }

        private void RandomElectionTimeout()
        {
            var random = new Random();
            _electionTimeout = random.Next(800, 2000);
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
                //_logger.LogInformation($"Node is now in '{_state.GetType().Name}' state.");
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

        public int ElectionTimeout
        {   
            get { return _electionTimeout; }
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