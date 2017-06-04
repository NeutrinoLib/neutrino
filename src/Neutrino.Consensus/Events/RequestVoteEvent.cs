using Neutrino.Consensus.Entities;
using Neutrino.Consensus.Events;

namespace Neutrino.Consensus.Events
{
    public class RequestVoteEvent : IEvent
    {
        public int Term { get; set; }
        
        public NodeInfo Node { get; set; }

        public RequestVoteEvent()
        {
        }

        public RequestVoteEvent(int term, NodeInfo node)
        {
            Term = term;
            Node = node;
        }
    }
}