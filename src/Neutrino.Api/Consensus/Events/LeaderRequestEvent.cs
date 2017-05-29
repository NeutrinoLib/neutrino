using Neutrino.Api.Consensus.Events;
using Neutrino.Entities;

namespace Neutrino.Api.Consensus.Events
{
    public class LeaderRequestEvent : IEvent
    {
        public int CurrentTerm { get; set; }
        
        public Node Node { get; set; }

        public LeaderRequestEvent(int currentTerm, Node node)
        {
            CurrentTerm = currentTerm;
            Node = node;
        }
    }
}