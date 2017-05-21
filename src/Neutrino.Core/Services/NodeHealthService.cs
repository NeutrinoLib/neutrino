using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neutrino.Core.Infrastructure;
using Neutrino.Core.Repositories;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public class NodeHealthService : INodeHealthService
    {
        private readonly IRepository<NodeHealth> _nodeHealthRepository;

        public NodeHealthService(IRepository<NodeHealth> nodeHealthRepository)
        {
            _nodeHealthRepository = nodeHealthRepository;
        }

        public IEnumerable<NodeHealth> Get(string nodeId)
        {
            var nodeHealth = _nodeHealthRepository.Get(x => x.NodeId == nodeId);
            return nodeHealth;
        }

        public NodeHealth GetCurrent(string nodeId)
        {
            var healtList = _nodeHealthRepository.Get(x => x.NodeId == nodeId).ToList();
            var currentHealth = healtList.OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            return currentHealth;
        }

        public NodeHealth Get(string nodeId, string id)
        {
            var query = _nodeHealthRepository.Get(x => x.NodeId == nodeId && x.Id == id);
            return query.FirstOrDefault();
        }

        public void Create(string nodeId, NodeHealth nodeHealth)
        {
            nodeHealth.NodeId = nodeId;
            nodeHealth.CreatedDate = DateTime.UtcNow;
            _nodeHealthRepository.Create(nodeHealth);
        }
    }
}