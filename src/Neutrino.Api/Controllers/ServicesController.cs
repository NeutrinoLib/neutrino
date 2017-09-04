using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Consensus;
using Neutrino.Consensus.States;
using Neutrino.Core.Services;
using Neutrino.Entities.Model;

namespace Neutrino.Api.Controllers
{
    /// <summary>
    /// Services controller.
    /// </summary>
    [Authorize]
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
        /// <param name="consensusContext">Context of consensus protocol.</param>
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
        /// Returns all registered services.
        /// </summary>
        /// <remarks>
        /// Endpoint returns all registered services.
        /// </remarks>
        /// <returns>List of services.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Service>))]
        public IEnumerable<Service> Get([FromQuery] string serviceType = null, [FromQuery] string[] tags = null)
        {
            var services = _servicesService.Get(serviceType, tags);
            return services;
        }

        /// <summary>
        /// Returns service by id.
        /// </summary>
        /// <remarks>
        /// Endpoint returns specific service information.
        /// </remarks>
        /// <param name="id">Service id.</param>
        /// <returns>Specific service information.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Service))]
        [ProducesResponseType(404)]
        public ActionResult Get(string id)
        {
            var service = _servicesService.Get(id);
            if(service == null)
            {
                return NotFound();
            }

            return new ObjectResult(service);
        }

        /// <summary>
        /// Creates new service.
        /// </summary>
        /// <remarks>
        /// Endpoint for registering new service information.
        /// </remarks>
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
        /// <remarks>
        /// Endpoint for updating service information.
        /// </remarks>
        /// <param name="id">Service id to update.</param>
        /// <param name="service">New service information.</param>
        /// <returns>Returns 200 (Ok) if update was finished successfully.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> Put(string id, [FromBody]Service service)
        {
            if(!_consensusContext.IsLeader())
            {
                return Redirect($"{_consensusContext.NodeVote.LeaderNode.Address}/api/services");
            }

            var serviceFromStore = _servicesService.Get(id);
            if(serviceFromStore == null)
            {
                return NotFound();
            }

            service.Id = id;
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
        /// <remarks>
        /// Endpoint for deleting service.
        /// </remarks>
        /// <param name="id">Service id.</param>
        /// <returns>Returns 200 (Ok) if service was deleted.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(string id)
        {
            if(!_consensusContext.IsLeader())
            {
                return Redirect($"{_consensusContext.NodeVote.LeaderNode.Address}/api/services");
            }

            var service = _servicesService.Get(id);
            if(service == null)
            {
                return NotFound();
            }

            var actionConfirmation = await _servicesService.Delete(id);
            if(actionConfirmation.WasSuccessful)
            {
                return Ok();
            }
            else
            {
                return BadRequest(actionConfirmation);
            }
        }
    }
}