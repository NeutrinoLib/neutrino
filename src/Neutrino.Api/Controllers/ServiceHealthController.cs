using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Core.Services;
using Neutrino.Entities.List;
using Neutrino.Entities.Model;

namespace Neutrino.Api.Controllers
{
    /// <summary>
    /// Services health controller.
    /// </summary>
    [Authorize]
    [Route("api/service-health")]
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
        /// <remarks>
        /// Endpoint returns all health information for specific service.
        /// </remarks>
        /// <param name="serviceId">Service id.</param>
        /// <param name="offset">Offset. If not specified, it starts from 0.</param>
        /// <param name="limit">Limit. If not specified returns all rows.</param>
        /// <returns>Returns service's health information.</returns>
        [HttpGet("{serviceId}")]
        [ProducesResponseType(200, Type = typeof(PageList<ServiceHealth>))]
        public PageList<ServiceHealth> GetServiceHealth(string serviceId, [FromQuery] int offset = 0, [FromQuery] int limit = Int32.MaxValue)
        {
            var serviceHealth = _serviceHealthService.Get(serviceId, offset, limit);
            return serviceHealth;
        }

        /// <summary>
        /// Returns current service health.
        /// </summary>
        /// <remarks>
        /// Endpoint returns only current healt status for specific endpoint.
        /// </remarks>
        /// <param name="serviceId">Service id.</param>
        /// <returns>Returns current service's health information.</returns>
        [HttpGet("{serviceId}/current")]
        [ProducesResponseType(200, Type = typeof(ServiceHealth))]
        public ServiceHealth GetCurrentServiceHealth(string serviceId)
        {
            var serviceHealth = _serviceHealthService.GetCurrent(serviceId);
            return serviceHealth;
        }
    }
}