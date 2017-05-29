using Neutrino.Entities;

namespace Neutrino.Core.Services.Parameters
{
    public class ApplicationParameters
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public Node[] Nodes { get; set; }

        public Node CurrentNode { get; set; }
    }
}