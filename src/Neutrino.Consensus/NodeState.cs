using Neutrino.Entities;

namespace Neutrino.Api.Consensus
{
    public class NodeState
    {
        public Node Node { get; set; }

        public bool VoteGranted { get; set; }
    }
}