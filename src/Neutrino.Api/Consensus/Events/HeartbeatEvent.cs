using Neutrino.Entities;

namespace Neutrino.Api.Consensus.Events
{
    public class HeartbeatEvent : IEvent
    {
        public int CurrentTerm { get; set; }

        public Node Node { get; set; }

        public HeartbeatEvent(int currentTerm, Node node)
        {
            CurrentTerm = currentTerm;
            Node = node;
        }
    }
}