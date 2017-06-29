using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Consensus;
using Neutrino.Core.Services;
using Neutrino.Entities;

namespace Neutrino.Api.Controllers
{
    /// <summary>
    /// Key-value properties controller.
    /// </summary>
    [Route("api/key-values")]
    public class KvPropertiesController : Controller
    {
        private readonly IKvPropertyService _kvPropertyService;
        private readonly IConsensusContext _consensusContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="kvPropertyService">Key-value property service.</param>
        /// /// <param name="consensusContext">Consensus context.</param>
        public KvPropertiesController(
            IKvPropertyService kvPropertyService,
            IConsensusContext consensusContext)
        {
            _kvPropertyService = kvPropertyService;
            _consensusContext = consensusContext;
        }

        /// <summary>
        /// Returns all key-value properties.
        /// </summary>
        /// <remarks>
        /// Endpoint returns all registered key-value properties.
        /// </remarks>
        /// <returns>List of key-value properties.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<KvProperty>))]
        public IEnumerable<KvProperty> Get()
        {
            var services = _kvPropertyService.Get();
            return services;
        }

        /// <summary>
        /// Returns key-value item by key.
        /// </summary>
        /// <remarks>
        /// Endpoint returns specific key-value property by key.
        /// </remarks>
        /// <param name="key">Key of item.</param>
        /// <returns>Specific key-value property.</returns>
        [HttpGet("{key}")]
        [ProducesResponseType(200, Type = typeof(KvProperty))]
        [ProducesResponseType(404)]
        public ActionResult Get(string key)
        {
            var service = _kvPropertyService.Get(key);
            if(service == null)
            {
                return NotFound();
            }

            return new ObjectResult(service);
        }

        /// <summary>
        /// Creates new key-value property..
        /// </summary>
        /// <remarks>
        /// Endpoint for creating new key-value property.
        /// </remarks>
        /// <param name="kvProperty">Key-value property.</param>
        /// <returns>Returns 201 (Created) if key-value was created.</returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(KvProperty))]
        [ProducesResponseType(400)]
        public async Task<ActionResult> Post([FromBody]KvProperty kvProperty)
        {
            if(!_consensusContext.IsLeader())
            {
                return Redirect($"{_consensusContext.NodeVote.LeaderNode.Address}/api/key-values");
            }

            var actionConfirmation = await _kvPropertyService.Create(kvProperty);
            if(actionConfirmation.WasSuccessful)
            {
                return Created($"api/key-values/{kvProperty.Key}", kvProperty);
            }
            else
            {
                return BadRequest(actionConfirmation);
            }
        }

        /// <summary>
        /// Updates key-value property.
        /// </summary>
        /// <remarks>
        /// Endpoint for updating key-value property.
        /// </remarks>
        /// <param name="key">Key-value property key to update.</param>
        /// <param name="kvProperty">New key-value property.</param>
        /// <returns>Returns 200 (Ok) if update was finished successfully.</returns>
        [HttpPut("{key}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> Put(string key, [FromBody]KvProperty kvProperty)
        {
            if(!_consensusContext.IsLeader())
            {
                return Redirect($"{_consensusContext.NodeVote.LeaderNode.Address}/api/key-values");
            }

            var serviceFromStore = _kvPropertyService.Get(key);
            if(serviceFromStore == null)
            {
                return NotFound();
            }

            kvProperty.Key = key;
            var actionConfirmation = await _kvPropertyService.Update(kvProperty);
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
        /// Deletes key-value property.
        /// </summary>
        /// <remarks>
        /// Endpoint for deleting key-value property.
        /// </remarks>
        /// <param name="key">Key-value property key.</param>
        /// <returns>Returns 200 (Ok) if service was deleted.</returns>
        [HttpDelete("{key}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(string key)
        {
            if(!_consensusContext.IsLeader())
            {
                return Redirect($"{_consensusContext.NodeVote.LeaderNode.Address}/api/key-values");
            }

            var service = _kvPropertyService.Get(key);
            if(service == null)
            {
                return NotFound();
            }

            var actionConfirmation = await _kvPropertyService.Delete(key);
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