using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Api.Consensus.Events;
using Neutrino.Entities;

namespace Neutrino.Api.Consensus
{
    [Route("api/consensus")]
    public class NodesController : Controller
    {
        IConsensusContext _consensusContext;

        public NodesController(IConsensusContext consensusContext)
        {
            _consensusContext = consensusContext;
        }

        [HttpPost("heartbeat")]
        public ActionResult Heartbeat([FromBody] HeartbeatEvent heartbeatEvent)
        {
            _consensusContext.TriggerEvent(heartbeatEvent);
            return Ok();
        }

        [HttpPost("leader")]
        public ActionResult LeaderRequestVote([FromBody] LeaderRequestEvent leaderRequest)
        {
            var response = _consensusContext.TriggerEvent(leaderRequest);
            return new ObjectResult(response);
        }
    }
}