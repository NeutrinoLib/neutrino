using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Neutrino.Core.Infrastructure;
using Neutrino.Entities;
using System.Threading;
using Neutrino.Core.Repositories;
using Neutrino.Core.Diagnostics.Exceptions;
using Neutrino.Core.Services.Validators;

namespace Neutrino.Core.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IRepository<Service> _serviceRepository;

        private readonly IServiceValidator _serviceValidator;

        private readonly IMemoryCache _memoryCache;

        private readonly IHealthService _healthService;

        public ServicesService(
            IRepository<Service> serviceRepository,
            IServiceValidator serviceValidator,
            IMemoryCache memoryCache, 
            IHealthService healthService)
        {
            _serviceRepository = serviceRepository;
            _serviceValidator = serviceValidator;
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

        public ActionConfirmation Create(Service service)
        {
            var validatorConfirmation = _serviceValidator.Validate(service, ActionType.Create);
            if(!validatorConfirmation.WasSuccessful)
            {
                return validatorConfirmation;
            }

            _serviceRepository.Create(service);

            if(service.HealthCheck != null && service.HealthCheck.HealthCheckType == HealthCheckType.HttpRest)
            {
                _healthService.RunHealthChecker(service);
            }

            return ActionConfirmation.CreateSuccessful();
        }

        public ActionConfirmation Update(Service service)
        {
            var validatorConfirmation = _serviceValidator.Validate(service, ActionType.Update);
            if(!validatorConfirmation.WasSuccessful)
            {
                return validatorConfirmation;
            }

            _serviceRepository.Update(service);
            _healthService.StopHealthChecker(service.Id);

            if(service.HealthCheck != null && service.HealthCheck.HealthCheckType == HealthCheckType.HttpRest)
            {
                _healthService.RunHealthChecker(service);
            }

            return ActionConfirmation.CreateSuccessful();
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