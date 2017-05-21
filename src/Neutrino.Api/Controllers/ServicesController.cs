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
        public Service Get(string serviceId)
        {
            var service = _servicesService.Get(serviceId);
            return service;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        public ActionResult Post([FromBody]Service service)
        {
            var actionConfirmation = _servicesService.Create(service);
            if(actionConfirmation.WasSeccessful)
            {
                return Created($"api/services/{service.Id}", service);
            }
            else
            {
                return BadRequest(new { error = actionConfirmation.Message });
            }
        }

        [HttpPut("{serviceId}")]
        [ProducesResponseType(200)]
        public ActionResult Put(string serviceId, [FromBody]Service service)
        {
            _servicesService.Update(serviceId, service);
            return Ok();
        }

        [HttpDelete("{serviceId}")]
        [ProducesResponseType(200)]
        public ActionResult Delete(string serviceId)
        {
            _servicesService.Delete(serviceId);
            return Ok();
        }
    }
}