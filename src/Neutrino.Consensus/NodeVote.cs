using Neutrino.Entities;

namespace Neutrino.Api.Consensus
{
    public class NodeVote
    {
        public int VoteTerm { get; set; }
        public Node LeaderNode { get; set; }
    }
}