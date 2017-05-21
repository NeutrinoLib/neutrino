using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neutrino.Core.Infrastructure;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public class ServiceHealthService : IServiceHealthService
    {
        private readonly IStoreContext _storeContext;

        private readonly IHealthService _healthService;

        public ServiceHealthService(IStoreContext storeContext, IHealthService healthService)
        {
            _storeContext = storeContext;
            _healthService = healthService;
        }

        public IEnumerable<ServiceHealth> Get(string serviceId)
        {
            var query = _storeContext.Repository.Query<ServiceHealth>().Where(x => x.ServiceId == serviceId);
            return query.ToEnumerable();
        }

        public ServiceHealth GetCurrent(string serviceId)
        {
            var serviceHealth = _healthService.GetServiceHealth(serviceId);
            return serviceHealth;
        }

        public ServiceHealth Get(string serviceId, string id)
        {
            var query = _storeContext.Repository.Query<ServiceHealth>().Where(x => x.ServiceId == serviceId && x.Id == id);
            return query.FirstOrDefault();
        }

        public void Create(string serviceId, ServiceHealth serviceHealth)
        {
            serviceHealth.ServiceId = serviceId;
            serviceHealth.CreatedDate = DateTime.UtcNow;
            _storeContext.Repository.Insert(serviceHealth);
        }
    }
}