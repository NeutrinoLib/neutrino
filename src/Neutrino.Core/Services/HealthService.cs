using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Neutrino.Core.Infrastructure;
using Neutrino.Core.Repositories;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public class HealthService : IHealthService
    {
        private readonly IRepository<Service> _serviceRepository;

        private readonly IRepository<ServiceHealth> _serviceHealthRepository;

        private readonly IDictionary<string, CancellationTokenSource> _tokenSources;

        private readonly ILogger<HealthService> _logger;

        private readonly IMemoryCache _memoryCache;

        private readonly IApplicationLifetime _applicationLifetime;

        private readonly HttpClient _httpClient;

        public HealthService(
            IRepository<Service> serviceRepository,
            IRepository<ServiceHealth> serviceHealthRepository, 
            ILogger<HealthService> logger, 
            IMemoryCache memoryCache,
            IApplicationLifetime applicationLifetime,
            HttpClient httpClient)
        {
            _serviceRepository = serviceRepository;
            _serviceHealthRepository = serviceHealthRepository;
            _logger = logger;
            _memoryCache = memoryCache;
            _applicationLifetime = applicationLifetime;
            _httpClient = httpClient;

            _applicationLifetime.ApplicationStopping.Register(DisposeResources);
            _tokenSources = new ConcurrentDictionary<string, CancellationTokenSource>();
        }

        public void RunHealthChecker(Service service)
        {
            var key = GetKey(service.Id);
            var serviceHealth = new ServiceHealth
            {
                ServiceId = service.Id,
                HealthState = HealthState.Unknown
            };

            _memoryCache.Set(key, serviceHealth);

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            _tokenSources.Add(key, tokenSource);

            Task.Run(() => CheckHealthTask(service, serviceHealth, tokenSource.Token), tokenSource.Token);
        }

        public void StopHealthChecker(string serviceId)
        {
            var key = GetKey(serviceId);

            CancellationTokenSource tokenSource = null;
            if(_tokenSources.TryGetValue(key, out tokenSource))
            {
                tokenSource.Cancel();
            }
        }

        public ServiceHealth GetServiceHealth(string serviceId)
        {
            var key = GetKey(serviceId);
            ServiceHealth serviceHealth = null;
            if(_memoryCache.TryGetValue(key, out serviceHealth))
            {
                return serviceHealth;
            }

            return new ServiceHealth();
        }

        private async Task CheckHealthTask(Service service, ServiceHealth serviceHealth, CancellationToken token)
        {
            var interval = service.HealthCheck.Interval * 1000;
            var deregisterServiceTime = service.HealthCheck.DeregisterCriticalServiceAfter * 1000;
            int lastExecute = interval;
            int criticalTime = 0;

            while(!token.IsCancellationRequested) 
            {
                try
                {
                    if(lastExecute == interval)
                    {
                        lastExecute = 0;
                        await CheckHealth(service, serviceHealth);
                        criticalTime = 0;
                    }
                }
                catch(Exception exception)
                {
                    CatchHealthError(service, serviceHealth, exception);
                    criticalTime += interval;
                }
                finally
                {
                    Thread.Sleep(500);
                    lastExecute += 500;
                }

                if(criticalTime >= deregisterServiceTime)
                {
                    DeregisterService(service);
                    break;
                }
            }
        }

        private void DeregisterService(Service service)
        {
            _serviceRepository.Delete(service.Id);
            _logger.LogError($"Service '{service.Id}' is in critical state too long. Deregistering services.");

            var key = GetKey(service.Id);
            _tokenSources.Remove(key);
        }

        private void CatchHealthError(Service service, ServiceHealth serviceHealth, Exception exception)
        {
            serviceHealth.ResponseMessage = exception.Message;
            serviceHealth.HealthState = HealthState.Critical;
            serviceHealth.StatusCode = 0;
            serviceHealth.CreatedDate = DateTime.UtcNow;

            AddServiceHealthToStore(serviceHealth);

            _logger.LogError($"Health state of service '{service.Id}': {serviceHealth.HealthState}. Status code: {serviceHealth.StatusCode}. Message: '{serviceHealth.ResponseMessage}'.");
        }

        private async Task CheckHealth(Service service, ServiceHealth serviceHealth)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, service.HealthCheck.Address);
            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            var responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();

            serviceHealth.ResponseMessage = responseMessage;
            serviceHealth.HealthState = httpResponseMessage.IsSuccessStatusCode ? HealthState.Passing : HealthState.Error;
            serviceHealth.StatusCode = (int)httpResponseMessage.StatusCode;
            serviceHealth.CreatedDate = DateTime.UtcNow;

            AddServiceHealthToStore(serviceHealth);

            _logger.LogInformation($"Health state of service '{service.Id}': {serviceHealth.HealthState}. Status code: {serviceHealth.StatusCode}. Message: '{serviceHealth.ResponseMessage}'.");
        }

        private void AddServiceHealthToStore(ServiceHealth serviceHealth)
        {
            var newServiceHealth = new ServiceHealth
            {
                Id = Guid.NewGuid().ToString(),
                ResponseMessage = serviceHealth.ResponseMessage,
                HealthState = serviceHealth.HealthState,
                StatusCode = serviceHealth.StatusCode,
                CreatedDate = serviceHealth.CreatedDate,
                ServiceId = serviceHealth.ServiceId
            };

            _serviceHealthRepository.Create(newServiceHealth);
        }

        private string GetKey(string serviceId)
        {
            return $"service-health-{serviceId}";
        }

        protected void DisposeResources()
        {
            Console.WriteLine("Canceling health events...");
            foreach(var tokenSource in _tokenSources.Values)
            {
                tokenSource.Cancel();
            }
        }
    }
}