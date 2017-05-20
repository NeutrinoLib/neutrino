using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public interface INodeHealthService
    {
        IEnumerable<NodeHealth> Get(string nodeId);

        NodeHealth GetCurrent(string nodeId);

        NodeHealth Get(string nodeId, string id);

        void Create(string nodeId, NodeHealth nodeHealth);
    }
}