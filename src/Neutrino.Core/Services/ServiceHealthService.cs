using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neutrino.Core.Infrastructure;
using Neutrino.Core.Repositories;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public class ServiceHealthService : IServiceHealthService
    {
        private readonly IRepository<ServiceHealth> _serviceHealthRepository;

        private readonly IHealthService _healthService;

        public ServiceHealthService(IRepository<ServiceHealth> serviceHealthRepository, IHealthService healthService)
        {
            _serviceHealthRepository = serviceHealthRepository;
            _healthService = healthService;
        }

        public IEnumerable<ServiceHealth> Get()
        {
            var serviceHealth = _serviceHealthRepository.Get();
            return serviceHealth;
        }

        public IEnumerable<ServiceHealth> Get(string serviceId)
        {
            var serviceHealth = _serviceHealthRepository.Get(x => x.ServiceId == serviceId);
            return serviceHealth;
        }

        public ServiceHealth GetCurrent(string serviceId)
        {
            var serviceHealth = _healthService.GetServiceHealth(serviceId);
            return serviceHealth;
        }

        public ServiceHealth Get(string serviceId, string id)
        {
            var query = _serviceHealthRepository.Get(x => x.ServiceId == serviceId && x.Id == id);
            return query.FirstOrDefault();
        }

        public void Create(string serviceId, ServiceHealth serviceHealth)
        {
            serviceHealth.ServiceId = serviceId;
            _serviceHealthRepository.Create(serviceHealth);
        }

        public void Clear()
        {
            _serviceHealthRepository.Clear();
        }
    }
}