using System.Collections.Generic;
using Neutrino.Consensus.Entities;
using Neutrino.Consensus.Events;
using Neutrino.Consensus.Responses;
using Neutrino.Consensus.States;
using Neutrino.Consensus.Options;
using System.Net.Http;

namespace Neutrino.Consensus
{
    public interface IConsensusContext
    {
        int CurrentTerm { get; set; }

        int ElectionTimeout { get; }

        int HeartbeatTimeout { get; }

        State State { get; set; }

        NodeInfo CurrentNode { get; }

        IList<NodeState> NodeStates { get; }

        NodeVote NodeVote { get; }

        ILogReplicable LogReplicable { get; }

        IStateObservable StateObservable { get; }

        HttpClient HttpClient { get; }

        ConsensusOptions ConsensusOptions { get; }

        void Run(ConsensusOptions consensusOptions, IStateObservable stateObservable, ILogReplicable logReplicable);

        void EnsureLogConsistency();

        bool IsLeader();
    }
}