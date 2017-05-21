using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Core.Infrastructure;
using Neutrino.Core.Repositories;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public class NodesService : INodesService
    {
        private readonly IRepository<Node> _nodeRepository;

        public NodesService(IRepository<Node> nodeRepository)
        {
            _nodeRepository = nodeRepository;
        }

        public IEnumerable<Node> Get()
        {
            var nodes = _nodeRepository.Get();
            return nodes;
        }

        public Node Get(string id)
        {
            var node = _nodeRepository.Get(id);
            return node;
        }

        public void Create(Node node)
        {
            _nodeRepository.Create(node);
        }

        public void Update(string id, Node node)
        {
            _nodeRepository.Update(id, node);
        }

        public void Delete(string id)
        {
            _nodeRepository.Delete(id);
        }
    }
}