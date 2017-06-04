namespace Neutrino.Consensus.Entities
{
    public class NodeVote
    {
        public int VoteTerm { get; set; }
        public NodeInfo LeaderNode { get; set; }
    }
}