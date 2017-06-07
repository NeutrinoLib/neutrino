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
    [Route("api/services")]
    public class ServicesController : Controller
    {
        private readonly IServicesService _servicesService;
        private readonly IConsensusContext _consensusContext;

        public ServicesController(
            IServicesService servicesService,
            IConsensusContext consensusContext)
        {
            _servicesService = servicesService;
            _consensusContext = consensusContext;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Service>))]
        public IEnumerable<Service> Get()
        {
            var services = _servicesService.Get();
            return services;
        }

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
    }
}