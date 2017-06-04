using System.Collections.Generic;
using Neutrino.Consensus.Entities;

namespace Neutrino.Consensus.Events
{
    public class AppendEntriesEvent : IEvent
    {
        public int Term { get; set; }

        public NodeInfo LeaderNode { get; set; }

        public IList<Entry> Entries { get; set; }

        public AppendEntriesEvent()
        {
        }

        public AppendEntriesEvent(int term, NodeInfo leaderNode)
        {
            Term = term;
            LeaderNode = leaderNode;
        }

        public AppendEntriesEvent(int term, NodeInfo leaderNode, IList<Entry> entries) : this(term, leaderNode)
        {
            Entries = entries;
        }
    }
}