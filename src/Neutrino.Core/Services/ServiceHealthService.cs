using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neutrino.Core.Infrastructure;
using Neutrino.Core.Repositories;
using Neutrino.Entities.List;
using Neutrino.Entities.Model;

namespace Neutrino.Core.Services
{
    public class ServiceHealthService : IServiceHealthService
    {
        private readonly IRepository<ServiceHealth> _serviceHealthRepository;

        public ServiceHealthService(IRepository<ServiceHealth> serviceHealthRepository)
        {
            _serviceHealthRepository = serviceHealthRepository;
        }

        public IEnumerable<ServiceHealth> Get()
        {
            var serviceHealth = _serviceHealthRepository.Get();
            return serviceHealth;
        }

        public PageList<ServiceHealth> Get(string serviceId, int offset = 0, int limit = Int32.MaxValue)
        {
            var serviceHealthList = _serviceHealthRepository.Get(x => x.ServiceId == serviceId);
            var pageList = new PageList<ServiceHealth>()
            {
                AllRows = serviceHealthList.Count(),
                Limit = limit,
                Offset = offset,
                Rows = serviceHealthList.Skip(offset).Take(limit).ToList()
            };

            return pageList;
        }

        public ServiceHealth GetCurrent(string serviceId)
        {
            var serviceHealth = _serviceHealthRepository.Get(x => x.ServiceId == serviceId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if(serviceHealth == null)
            {
                serviceHealth = new ServiceHealth { HealthState = HealthState.Unknown };
            }

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