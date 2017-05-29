using System;
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
        private State _state;
        private readonly ApplicationParameters _applicationParameters;
        private readonly ILogger<ConsensusContext> _logger;
        private readonly IApplicationLifetime _applicationLifetime;

        public ConsensusContext(
            IOptions<ApplicationParameters> applicationParameters, 
            ILogger<ConsensusContext> logger,
            IApplicationLifetime applicationLifetime)
        {
            _applicationParameters = applicationParameters.Value;
            _logger = logger;
            _applicationLifetime = applicationLifetime;

            _applicationLifetime.ApplicationStopping.Register(DisposeResources);
        }

        public void Run()
        {
            State = new Follower(this);
        }

        public IResponse TriggerEvent(IEvent triggeredEvent)
        {
            return State.TriggerEvent(triggeredEvent);
        }

        public Node LeaderNode { get; set; }

        public int CurrentTerm { get; set; } = 1;

        public State State 
        {
            get { return _state; }
            set
            {
                _state = value;
                _logger.LogInformation($"Node is now in '{_state}' state.");

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

        public Node[] Nodes 
        {
            get 
            {
                return _applicationParameters.Nodes;
            }
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