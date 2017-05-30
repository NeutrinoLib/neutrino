using Neutrino.Entities;

namespace Neutrino.Core.Services.Parameters
{
    public class ApplicationParameters
    {
        public int MinElectionTimeout { get; set; }
        
        public int MaxElectionTimeout { get; set; }

        public int HeartbeatTimeout { get; set; }

        public ConnectionStrings ConnectionStrings { get; set; }

        public Node[] Nodes { get; set; }

        public Node CurrentNode { get; set; }
    }
}