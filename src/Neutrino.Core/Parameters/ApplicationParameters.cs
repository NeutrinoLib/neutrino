using Neutrino.Consensus.Entities;
using Neutrino.Entities;

namespace Neutrino.Core.Services.Parameters
{
    public class ApplicationParameters
    {
        public string SecureToken { get; set; }
        public int MinElectionTimeout { get; set; }
        
        public int MaxElectionTimeout { get; set; }

        public int HeartbeatTimeout { get; set; }

        public ConnectionStrings ConnectionStrings { get; set; }

        public NodeInfo[] Nodes { get; set; }

        public NodeInfo CurrentNode { get; set; }
    }
}