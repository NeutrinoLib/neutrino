using Neutrino.Entities;

namespace Neutrino.Consensus.Options
{
    public class ConsensusOptions
    {
        public Node[] Nodes { get; set; }

        public Node CurrentNode { get; set; }

        public int MinElectionTimeout { get; set; }
        
        public int MaxElectionTimeout { get; set; }

        public int HeartbeatTimeout { get; set; }
    }
}