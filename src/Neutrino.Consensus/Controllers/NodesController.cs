using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Consensus.Entities;

namespace Neutrino.Consensus.Controllers
{
    /// <summary>
    /// Controller for managing nodes information.
    /// </summary>
    [Route("api/nodes")]
    public class NodesController : Controller
    {
        private readonly IConsensusContext _consensusContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="consensusContext">Consensus context.</param>
        public NodesController(IConsensusContext consensusContext)
        {
            _consensusContext = consensusContext;
        }

        /// <summary>
        /// Returns list of all defined nodes.
        /// </summary>
        /// <remarks>
        /// Endpoint returns all nodes wich are registered in current node.
        /// </remarks>
        /// <returns>List of nodes.</returns>
        [HttpGet]
        public IEnumerable<NodeInfo> Get()
        {
            var nodes = _consensusContext.NodeStates.Select(x => x.Node);
            nodes = nodes.Append(_consensusContext.CurrentNode);

            return nodes;
        }

        /// <summary>
        /// Returns current node information.
        /// </summary>
        /// <remarks>
        /// Endpoint returns information about current node.
        /// </remarks>
        /// <returns>Information about current node.</returns>
        [HttpGet("current")]
        public NodeInfo GetCurrentNode()
        {
            return _consensusContext.CurrentNode;
        }

        /// <summary>
        /// Returns current node state.
        /// </summary>
        /// <remarks>
        /// Endpoint returns current node state. Node can be at one of following state:
        /// - Follower,
        /// - Candidate,
        /// - Leader.
        /// </remarks>
        /// <returns>Current node state.</returns>
        [HttpGet("current/state")]
        public JsonResult GetCurrentNodeState()
        {
            var state = _consensusContext.State.GetType().Name;
            return new JsonResult(new { state = state });
        }
    }
}