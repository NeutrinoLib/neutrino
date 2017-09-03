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
using Neutrino.Entities.Model;
using Neutrino.Core.Workers;
using Neutrino.Entities.Response;
using Neutrino.Consensus.Entities;
using System.Linq;

namespace Neutrino.Core.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IRepository<Service> _serviceRepository;

        private readonly IServiceValidator _serviceValidator;

        private readonly IMemoryCache _memoryCache;

        private readonly IServiceHealthWorker _serviceHealthWorker;

        private readonly ILogReplication _logReplication;

        private readonly IConsensusContext _consensusContext;

        public ServicesService(
            IRepository<Service> serviceRepository,
            IServiceValidator serviceValidator,
            IMemoryCache memoryCache, 
            IServiceHealthWorker serviceHealthWorker,
            ILogReplication logReplication,
            IConsensusContext consensusContext)
        {
            _serviceRepository = serviceRepository;
            _serviceValidator = serviceValidator;
            _memoryCache = memoryCache;
            _serviceHealthWorker = serviceHealthWorker;
            _logReplication = logReplication;
            _consensusContext = consensusContext;
        }

        public IEnumerable<Service> Get(string serviceType = null, string[] tags = null)
        {
            var services = _serviceRepository.Get();
            
            if(serviceType != null)
            {
                services = services.Where(x => x.ServiceType == serviceType);
            }

            if(tags != null && tags.Length > 0)
            {
                foreach(var tag in tags) 
                {
                    services = services.Where(x => x.Tags.Contains(tag));
                }
            }

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

            if(_consensusContext.IsLeader())
            {
                var consensusResult = await _logReplication.DistributeEntry(service, MethodType.Create);
                if(consensusResult.WasSuccessful)
                {
                    _serviceRepository.Create(service);
                    if (ShouldExecuteHealthChecking(service))
                    {
                        _serviceHealthWorker.RunHealthChecker(service);
                    }
                }
                else
                {
                    return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
                }
            }
            else 
            {
                _serviceRepository.Create(service);
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

            if(_consensusContext.IsLeader())
            {
                var consensusResult = await _logReplication.DistributeEntry(service, MethodType.Update);
                if(consensusResult.WasSuccessful)
                {
                    _serviceRepository.Update(service);
                    _serviceHealthWorker.StopHealthChecker(service.Id);

                    if(ShouldExecuteHealthChecking(service))
                    {
                        _serviceHealthWorker.RunHealthChecker(service);
                    }
                }
                else
                {
                    return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
                }
            }
            else
            {
                _serviceRepository.Update(service);
            }

            return ActionConfirmation.CreateSuccessful();
        }

        public async Task<ActionConfirmation> Delete(string id)
        {
            var service = _serviceRepository.Get(id);

            if(_consensusContext.IsLeader())
            {
                var consensusResult = await _logReplication.DistributeEntry(service, MethodType.Delete);
                if(consensusResult.WasSuccessful)
                {
                    _serviceRepository.Remove(id);
                    _serviceHealthWorker.StopHealthChecker(id);
                }
                else
                {
                    return ActionConfirmation.CreateError($"Distribute item to other services fails: '{consensusResult.Message}'.");
                }
            }
            else
            {
                _serviceRepository.Remove(id);
            }

            return ActionConfirmation.CreateSuccessful();
        }

        public void RunHealthChecker()
        {
            var services = _serviceRepository.Get(x => x.HealthCheck.HealthCheckType == HealthCheckType.HttpRest);
            foreach(var service in services)
            {
                _serviceHealthWorker.RunHealthChecker(service);
            }
        }

        public void StopHealthChecker()
        {
            var services = _serviceRepository.Get(x => x.HealthCheck.HealthCheckType == HealthCheckType.HttpRest);
            foreach(var service in services)
            {
                _serviceHealthWorker.StopHealthChecker(service.Id);
            }
        }

        public void Clear()
        {
            _serviceRepository.Clear();
        }

        private bool ShouldExecuteHealthChecking(Service service)
        {
            return _consensusContext.IsLeader() &&
                    service.HealthCheck != null &&
                    service.HealthCheck.HealthCheckType == HealthCheckType.HttpRest;
        }
    }
}