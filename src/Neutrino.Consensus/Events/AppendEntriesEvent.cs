using Neutrino.Consensus.Entities;

namespace Neutrino.Consensus.Events
{
    public class AppendEntriesEvent : IEvent
    {
        public int Term { get; set; }

        public NodeInfo LeaderNode { get; set; }

        public AppendEntriesEvent(int term, NodeInfo leaderNode)
        {
            Term = term;
            LeaderNode = leaderNode;
        }
    }
}