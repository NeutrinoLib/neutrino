using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Core.Infrastructure;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IStoreContext _storeContext;

        public ServicesService(IStoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public IEnumerable<Service> Get()
        {
            var query = _storeContext.Repository.Query<Service>();
            return query.ToEnumerable();
        }

        public Service Get(string id)
        {
            var query = _storeContext.Repository.Query<Service>().Where(x => x.Id == id);
            return query.FirstOrDefault();
        }

        public void Create(Service service)
        {
            service.CreatedDate = DateTime.UtcNow;
            _storeContext.Repository.Insert(service);
        }

        public void Update(string id, Service service)
        {
            service.Id = id;
            _storeContext.Repository.Update(service);
        }

        public void Delete(string id)
        {
            _storeContext.Repository.Delete<Service>(id);
        }
    }
}