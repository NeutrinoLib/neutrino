using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Core.Services;
using Neutrino.Entities;

namespace Neutrino.Api
{
    /// <summary>
    /// Service's health controller.
    /// </summary>
    [Route("api/services/{serviceId}/health")]
    public class ServiceHealthController : Controller
    {
        private readonly IServiceHealthService _serviceHealthService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceHealthService">Searvices health service.</param>
        public ServiceHealthController(IServiceHealthService serviceHealthService)
        {
            _serviceHealthService = serviceHealthService;
        }

        /// <summary>
        /// Returns service health.
        /// </summary>
        /// <param name="serviceId">Service id.</param>
        /// <returns>Returns service's health information.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ServiceHealth>))]
        public IEnumerable<ServiceHealth> Get(string serviceId)
        {
            var serviceHealth = _serviceHealthService.Get(serviceId);
            return serviceHealth;
        }

        /// <summary>
        /// Returns current service health.
        /// </summary>
        /// <param name="serviceId">Service id.</param>
        /// <returns>Returns current service's health information.</returns>
        [HttpGet("current")]
        [ProducesResponseType(200, Type = typeof(ServiceHealth))]
        public ServiceHealth GetCurrent(string serviceId)
        {
            var serviceHealth = _serviceHealthService.GetCurrent(serviceId);
            return serviceHealth;
        }
    }
}