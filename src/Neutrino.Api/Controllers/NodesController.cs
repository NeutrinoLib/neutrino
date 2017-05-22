using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Core.Services;
using Neutrino.Entities;

namespace Neutrino.Api.Controllers
{
    [Route("api/nodes")]
    public class NodesController : Controller
    {
        private readonly INodesService _nodesService;

        public NodesController(INodesService nodesService)
        {
            _nodesService = nodesService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Node>))]
        public IEnumerable<Node> Get()
        {
            var nodes = _nodesService.Get();
            return nodes;
        }

        [HttpGet("{nodeId}")]
        [ProducesResponseType(200, Type = typeof(Node))]
        public Node Get(string nodeId)
        {
            var node = _nodesService.Get(nodeId);
            return node;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        public ActionResult Post([FromBody]Node node)
        {
            _nodesService.Create(node);
            return Created($"api/nodes/{node.Id}", node);
        }

        [HttpPut("{nodeId}")]
        [ProducesResponseType(200)]
        public ActionResult Put(string nodeId, [FromBody]Node node)
        {
            node.Id = nodeId;
            _nodesService.Update(node);
            return Ok();
        }

        [HttpDelete("{nodeId}")]
        [ProducesResponseType(200)]
        public ActionResult Delete(string nodeId)
        {
            _nodesService.Delete(nodeId);
            return Ok();
        }
    }
}
