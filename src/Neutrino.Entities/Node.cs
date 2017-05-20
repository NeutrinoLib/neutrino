using System;

namespace Neutrino.Entities
{
    public class Node : BaseEntity
    {
        public string Name { get; set; }

        public string Datacenter { get; set; }

        public string Address { get; set; }
    }
}
