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
using Neutrino.Consensus;
using Neutrino.Consensus.Entities;

namespace Neutrino.Core.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IRepository<Service> _serviceRepository;

        private readonly IServiceValidator _serviceValidator;

        private readonly IMemoryCache _memoryCache;

        private readonly IHealthService _healthService;

        private readonly ILogReplication _logReplication;

        private readonly IConsensusContext _consensusContext;

        public ServicesService(
            IRepository<Service> serviceRepository,
            IServiceValidator serviceValidator,
            IMemoryCache memoryCache, 
            IHealthService healthService,
            ILogReplication logReplication,
            IConsensusContext consensusContext)
        {
            _serviceRepository = serviceRepository;
            _serviceValidator = serviceValidator;
            _memoryCache = memoryCache;
            _healthService = healthService;
            _logReplication = logReplication;
            _consensusContext = consensusContext;
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

        public async Task<ActionConfirmation> Create(Service service)
        {
            var validatorConfirmation = _serviceValidator.Validate(service, ActionType.Create);
            if(!validatorConfirmation.WasSuccessful)
            {
                return validatorConfirmation;
            }

            var consensusResult = await _logReplication.DistributeEntry(service, MethodType.Create);
            if(consensusResult.WasSuccessful)
            {
                _serviceRepository.Create(service);
                if (ShouldExecuteHealthChecking(service))
                {
                    _healthService.RunHealthChecker(service);
                }
            }
            else
            {
                return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
            }

            return ActionConfirmation.CreateSuccessful();
        }

        public async Task<ActionConfirmation> Update(Service service)
        {
            var validatorConfirmation = _serviceValidator.Validate(service, ActionType.Update);
            if(!validatorConfirmation.WasSuccessful)
            {
                return validatorConfirmation;
            }

            var consensusResult = await _logReplication.DistributeEntry(service, MethodType.Update);
            if(consensusResult.WasSuccessful)
            {
                _serviceRepository.Update(service);
                _healthService.StopHealthChecker(service.Id);

                if(ShouldExecuteHealthChecking(service))
                {
                    _healthService.RunHealthChecker(service);
                }
            }
            else
            {
                return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
            }

            return ActionConfirmation.CreateSuccessful();
        }

        public async Task<ActionConfirmation> Delete(string id)
        {
            var service = _serviceRepository.Get(id);
            var consensusResult = await _logReplication.DistributeEntry(service, MethodType.Delete);
            if(consensusResult.WasSuccessful)
            {
                _serviceRepository.Delete(id);
                _healthService.StopHealthChecker(id);
            }
            else
            {
                return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
            }

            return ActionConfirmation.CreateSuccessful();
        }

        public void RunHealthChecker()
        {
            var services = _serviceRepository.Get(x => x.HealthCheck.HealthCheckType == HealthCheckType.HttpRest);
            foreach(var service in services)
            {
                _healthService.RunHealthChecker(service);
            }
        }

        public void StopHealthChecker()
        {
            var services = _serviceRepository.Get(x => x.HealthCheck.HealthCheckType == HealthCheckType.HttpRest);
            foreach(var service in services)
            {
                _healthService.StopHealthChecker(service.Id);
            }
        }

        private bool ShouldExecuteHealthChecking(Service service)
        {
            return _consensusContext.IsLeader() &&
                    service.HealthCheck != null &&
                    service.HealthCheck.HealthCheckType == HealthCheckType.HttpRest;
        }
    }
}