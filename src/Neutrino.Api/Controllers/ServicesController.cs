using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Core.Services;
using Neutrino.Entities;

namespace Neutrino.Api
{
    [Route("api/services")]
    public class ServicesController : Controller
    {
        private readonly IServicesService _servicesService;

        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
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
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public ActionResult Post([FromBody]Service service)
        {
            var actionConfirmation = _servicesService.Create(service);
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
        public ActionResult Put(string serviceId, [FromBody]Service service)
        {
            var serviceFromStore = _servicesService.Get(serviceId);
            if(serviceFromStore == null)
            {
                return NotFound();
            }

            service.Id = serviceId;
            var actionConfirmation = _servicesService.Update(service);
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
        public ActionResult Delete(string serviceId)
        {
            var service = _servicesService.Get(serviceId);
            if(service == null)
            {
                return NotFound();
            }

            _servicesService.Delete(serviceId);
            return Ok();
        }
    }
}