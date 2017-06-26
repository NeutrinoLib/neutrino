using System.Collections.Generic;

namespace Neutrino.Consensus.Entities
{
    public class NodeInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public IDictionary<string, string> Tags { get; set; }
    }
}