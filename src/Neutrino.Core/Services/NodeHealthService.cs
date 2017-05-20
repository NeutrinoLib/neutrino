using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neutrino.Core.Infrastructure;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public class NodeHealthService : INodeHealthService
    {
        private readonly IStoreContext _storeContext;

        public NodeHealthService(IStoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public IEnumerable<NodeHealth> Get(string nodeId)
        {
            var query = _storeContext.Repository.Query<NodeHealth>().Where(x => x.NodeId == nodeId);
            return query.ToEnumerable();
        }

        public NodeHealth GetCurrent(string nodeId)
        {
            var healtList = _storeContext.Repository.Query<NodeHealth>().Where(x => x.NodeId == nodeId).ToList();
            var currentHealth = healtList.OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            return currentHealth;
        }

        public NodeHealth Get(string nodeId, string id)
        {
            var query = _storeContext.Repository.Query<NodeHealth>().Where(x => x.NodeId == nodeId && x.Id == id);
            return query.FirstOrDefault();
        }

        public void Create(string nodeId, NodeHealth nodeHealth)
        {
            nodeHealth.NodeId = nodeId;
            nodeHealth.CreatedDate = DateTime.UtcNow;
            _storeContext.Repository.Insert(nodeHealth);
        }
    }
}