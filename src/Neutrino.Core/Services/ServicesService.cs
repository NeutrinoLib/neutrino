using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Neutrino.Core.Infrastructure;
using Neutrino.Entities;
using System.Threading;

namespace Neutrino.Core.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IStoreContext _storeContext;

        private readonly IMemoryCache _memoryCache;

        private readonly IHealthService _healthService;

        public ServicesService(IStoreContext storeContext, IMemoryCache memoryCache, IHealthService healthService)
        {
            _storeContext = storeContext;
            _memoryCache = memoryCache;
            _healthService = healthService;
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

            if(service.HealthCheck != null && service.HealthCheck.HealthCheckType == HealthCheckType.HttpRest)
            {
                _healthService.RunHealthChecker(service);
            }
        }

        public void Update(string id, Service service)
        {
            service.Id = id;
            _storeContext.Repository.Update(service);
        }

        public void Delete(string id)
        {
            _storeContext.Repository.Delete<Service>(id);
            _healthService.StopHealthChecker(id);
        }

        public void RunHealthChecker()
        {
            var query = _storeContext.Repository.Query<Service>()
                .Where(x => x.HealthCheck.HealthCheckType == HealthCheckType.HttpRest);

            foreach(var service in query.ToEnumerable())
            {
                _healthService.RunHealthChecker(service);
            }
        }
    }
}