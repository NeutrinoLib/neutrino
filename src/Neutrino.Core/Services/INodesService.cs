using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public interface INodesService
    {
        IEnumerable<Node> Get();

        Node Get(string id);

        void Create(Node node);

        void Update(Node node);

        void Delete(string id);
    }
}