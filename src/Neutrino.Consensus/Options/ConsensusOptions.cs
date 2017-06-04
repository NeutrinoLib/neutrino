using System.Collections.Generic;
using Neutrino.Consensus.Entities;

namespace Neutrino.Consensus.Options
{
    public class ConsensusOptions
    {
        public IList<NodeInfo> Nodes { get; set; }

        public NodeInfo CurrentNode { get; set; }

        public int MinElectionTimeout { get; set; }
        
        public int MaxElectionTimeout { get; set; }

        public int HeartbeatTimeout { get; set; }
    }
}