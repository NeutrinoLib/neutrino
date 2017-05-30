using System.Collections.Generic;
using Neutrino.Api.Consensus.Events;
using Neutrino.Api.Consensus.Responses;
using Neutrino.Api.Consensus.States;
using Neutrino.Entities;

namespace Neutrino.Api.Consensus
{
    public interface IConsensusContext
    {
        Node LeaderNode { get; set; }

        int CurrentTerm { get; set; }

        int ElectionTimeout { get; }

        State State { get; set; }

        Node CurrentNode { get; }

        IList<NodeState> NodeStates { get; }

        void Run();

        IResponse TriggerEvent(IEvent triggeredEvent);
    }
}