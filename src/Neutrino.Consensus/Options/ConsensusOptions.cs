using System;
using System.Collections.Generic;
using Neutrino.Consensus.Entities;
using Neutrino.Consensus.Events;
using Neutrino.Consensus.States;

namespace Neutrino.Consensus.Options
{
    public class ConsensusOptions
    {
        public IList<NodeInfo> Nodes { get; set; }

        public NodeInfo CurrentNode { get; set; }

        public int MinElectionTimeout { get; set; }
        
        public int MaxElectionTimeout { get; set; }

        public int HeartbeatTimeout { get; set; }

        public string AuthenticationParameter { get; set; }

        public string AuthenticationScheme { get; set; }
    }
}