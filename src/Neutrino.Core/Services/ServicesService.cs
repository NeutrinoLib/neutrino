using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Neutrino.Core.Infrastructure;
using Neutrino.Entities;
using System.Threading;
using Neutrino.Core.Repositories;

namespace Neutrino.Core.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IRepository<Service> _serviceRepository;

        private readonly IMemoryCache _memoryCache;

        private readonly IHealthService _healthService;

        public ServicesService(IRepository<Service> serviceRepository, IMemoryCache memoryCache, IHealthService healthService)
        {
            _serviceRepository = serviceRepository;
            _memoryCache = memoryCache;
            _healthService = healthService;
        }

        public IEnumerable<Service> Get()
        {
            var services = _serviceRepository.Get();
            return services;
        }

        public Service Get(string id)
        {
            var service = _serviceRepository.Get(id);
            return service;
        }

        public void Create(Service service)
        {
            _serviceRepository.Create(service);

            if(service.HealthCheck != null && service.HealthCheck.HealthCheckType == HealthCheckType.HttpRest)
            {
                _healthService.RunHealthChecker(service);
            }
        }

        public void Update(string id, Service service)
        {
            _serviceRepository.Update(id, service);
        }

        public void Delete(string id)
        {
            _serviceRepository.Delete(id);
            _healthService.StopHealthChecker(id);
        }

        public void RunHealthChecker()
        {
            var services = _serviceRepository.Get(x => x.HealthCheck.HealthCheckType == HealthCheckType.HttpRest);
            foreach(var service in services)
            {
                _healthService.RunHealthChecker(service);
            }
        }
    }
}