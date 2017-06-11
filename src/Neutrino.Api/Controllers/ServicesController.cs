using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Consensus;
using Neutrino.Consensus.States;
using Neutrino.Core.Services;
using Neutrino.Entities;

namespace Neutrino.Api
{
    /// <summary>
    /// Services controller.
    /// </summary>
    [Route("api/services")]
    public class ServicesController : Controller
    {
        private readonly IServicesService _servicesService;
        private readonly IServiceHealthService _serviceHealthService;
        private readonly IConsensusContext _consensusContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="servicesService">Services service.</param>
        /// <param name="serviceHealthService">Searvices health service.</param>
        /// /// <param name="consensusContext">Context of consensus protocol.</param>
        public ServicesController(
            IServicesService servicesService,
            IServiceHealthService serviceHealthService,
            IConsensusContext consensusContext)
        {
            _servicesService = servicesService;
            _serviceHealthService = serviceHealthService;
            _consensusContext = consensusContext;
        }

        /// <summary>
        /// Returns all list of services.
        /// </summary>
        /// <remarks>
        /// Endpoint returns all registered services.
        /// </remarks>
        /// <returns>List of services.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Service>))]
        public IEnumerable<Service> Get()
        {
            var services = _servicesService.Get();
            return services;
        }

        /// <summary>
        /// Returns service by id.
        /// </summary>
        /// <param name="serviceId">Service id.</param>
        /// <returns>Specific service information.</returns>
        [HttpGet("{serviceId}")]
        [ProducesResponseType(200, Type = typeof(Service))]
        [ProducesResponseType(404)]
        public ActionResult Get(string serviceId)
        {
            var service = _servicesService.Get(serviceId);
            if(service == null)
            {
                return NotFound();
            }

            return new ObjectResult(service);
        }

        /// <summary>
        /// Creates new service.
        /// </summary>
        /// <param name="service">Service information.</param>
        /// <returns>Returns 201 (Created) if service was created.</returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Service))]
        [ProducesResponseType(400)]
        public async Task<ActionResult> Post([FromBody]Service service)
        {
            if(!_consensusContext.IsLeader())
            {
                return Redirect($"{_consensusContext.NodeVote.LeaderNode.Address}/api/services");
            }

            var actionConfirmation = await _servicesService.Create(service);
            if(actionConfirmation.WasSuccessful)
            {
                return Created($"api/services/{service.Id}", service);
            }
            else
            {
                return BadRequest(actionConfirmation);
            }
        }

        /// <summary>
        /// Updates service information.
        /// </summary>
        /// <param name="serviceId">Service id to update.</param>
        /// <param name="service">New service information.</param>
        /// <returns>Returns 200 (Ok) if update was finished successfully.</returns>
        [HttpPut("{serviceId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> Put(string serviceId, [FromBody]Service service)
        {
            if(!_consensusContext.IsLeader())
            {
                return Redirect($"{_consensusContext.NodeVote.LeaderNode.Address}/api/services");
            }

            var serviceFromStore = _servicesService.Get(serviceId);
            if(serviceFromStore == null)
            {
                return NotFound();
            }

            service.Id = serviceId;
            var actionConfirmation = await _servicesService.Update(service);
            if(actionConfirmation.WasSuccessful)
            {
                return Ok();
            }
            else
            {
                return BadRequest(actionConfirmation);
            }
        }
        
        /// <summary>
        /// Deletes service.
        /// </summary>
        /// <param name="serviceId">Service id.</param>
        /// <returns>Returns 200 (Ok) if service was deleted.</returns>
        [HttpDelete("{serviceId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(string serviceId)
        {
            if(!_consensusContext.IsLeader())
            {
                return Redirect($"{_consensusContext.NodeVote.LeaderNode.Address}/api/services");
            }

            var service = _servicesService.Get(serviceId);
            if(service == null)
            {
                return NotFound();
            }

            await _servicesService.Delete(serviceId);
            return Ok();
        }

        /// <summary>
        /// Returns service health.
        /// </summary>
        /// <param name="serviceId">Service id.</param>
        /// <returns>Returns service's health information.</returns>
        [HttpGet("{serviceId}/health")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ServiceHealth>))]
        public IEnumerable<ServiceHealth> GetServiceHealth(string serviceId)
        {
            var serviceHealth = _serviceHealthService.Get(serviceId);
            return serviceHealth;
        }

        /// <summary>
        /// Returns current service health.
        /// </summary>
        /// <param name="serviceId">Service id.</param>
        /// <returns>Returns current service's health information.</returns>
        [HttpGet("{serviceId}/health/current")]
        [ProducesResponseType(200, Type = typeof(ServiceHealth))]
        public ServiceHealth GetCurrentServiceHealth(string serviceId)
        {
            var serviceHealth = _serviceHealthService.GetCurrent(serviceId);
            return serviceHealth;
        }
    }
}