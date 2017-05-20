using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Core.Services;
using Neutrino.Entities;

namespace Neutrino.Api
{
    [Route("api/nodes/{nodeId}/health")]
    public class NodeHealthController : Controller
    {
        private readonly INodeHealthService _nodeHealthService;

        public NodeHealthController(INodeHealthService nodeHealthService)
        {
            _nodeHealthService = nodeHealthService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NodeHealth>))]
        public IEnumerable<NodeHealth> Get(string nodeId)
        {
            var nodeHealth = _nodeHealthService.Get(nodeId);
            return nodeHealth;
        }

        [HttpGet("current")]
        [ProducesResponseType(200, Type = typeof(NodeHealth))]
        public NodeHealth GetCurrent(string nodeId)
        {
            var nodeHealth = _nodeHealthService.GetCurrent(nodeId);
            return nodeHealth;
        }
    }
}