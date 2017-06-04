using System;
using System.Collections.Generic;
using Neutrino.Consensus.Entities;
using Neutrino.Consensus.Events;
using Neutrino.Consensus.States;

namespace Neutrino.Consensus.Options
{
    public class ConsensusOptions
    {
        public Action<State, State> OnStateChangingCallback { get; private set; }

        public Action<State, State> OnStateChangedCallback { get; private set; }

        public Func<AppendEntriesEvent, bool> OnLogReplicationCallback { get; private set; }

        public IList<NodeInfo> Nodes { get; set; }

        public NodeInfo CurrentNode { get; set; }

        public int MinElectionTimeout { get; set; }
        
        public int MaxElectionTimeout { get; set; }

        public int HeartbeatTimeout { get; set; }

        public void OnStateChanging(Action<State, State> callback)
        {
            OnStateChangingCallback = callback;
        }

        public void OnStateChanged(Action<State, State> callback)
        {
            OnStateChangedCallback = callback;
        }

        public void OnLogReplication(Func<AppendEntriesEvent, bool> callback)
        {
            OnLogReplicationCallback = callback;
        }
    }
}