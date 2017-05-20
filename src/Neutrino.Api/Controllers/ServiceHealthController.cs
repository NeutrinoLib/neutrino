using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Core.Services;
using Neutrino.Entities;

namespace Neutrino.Api
{
    [Route("api/services/{serviceId}/health")]
    public class ServiceHealthController : Controller
    {
        private readonly IServiceHealthService _serviceHealthService;

        public ServiceHealthController(IServiceHealthService serviceHealthService)
        {
            _serviceHealthService = serviceHealthService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ServiceHealth>))]
        public IEnumerable<ServiceHealth> Get(string serviceId)
        {
            var serviceHealth = _serviceHealthService.Get(serviceId);
            return serviceHealth;
        }

        [HttpGet("current")]
        [ProducesResponseType(200, Type = typeof(ServiceHealth))]
        public ServiceHealth GetCurrent(string serviceId)
        {
            var serviceHealth = _serviceHealthService.GetCurrent(serviceId);
            return serviceHealth;
        }
    }
}