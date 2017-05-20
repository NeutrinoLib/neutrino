using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Core.Infrastructure;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public class NodesService : INodesService
    {
        private readonly IStoreContext _storeContext;

        public NodesService(IStoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public IEnumerable<Node> Get()
        {
            var query = _storeContext.Repository.Query<Node>();
            return query.ToEnumerable();
        }

        public Node Get(string id)
        {
            var query = _storeContext.Repository.Query<Node>().Where(x => x.Id == id);
            return query.FirstOrDefault();
        }

        public void Create(Node node)
        {
            node.CreatedDate = DateTime.UtcNow;
            _storeContext.Repository.Insert(node);
        }

        public void Update(string id, Node node)
        {
            node.Id = id;
            _storeContext.Repository.Update(node);
        }

        public void Delete(string id)
        {
            _storeContext.Repository.Delete<Node>(id);
        }
    }
}