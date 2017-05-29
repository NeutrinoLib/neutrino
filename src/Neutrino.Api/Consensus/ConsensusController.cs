using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Entities;

namespace Neutrino.Api.Consensus
{
    [Route("api/consensus")]
    public class NodesController : Controller
    {
        ILeaderElectionService _leaderElectionService;

        public NodesController(ILeaderElectionService leaderElectionService)
        {
            _leaderElectionService = leaderElectionService;
        }

        [HttpPost("heartbeat")]
        public ActionResult Heartbeat([FromBody] Node node)
        {
            _leaderElectionService.ReceiveHearbeat(node);
            return Ok();
        }

        [HttpPost("leader")]
        public ActionResult LeaderRequestVote([FromBody] Node node)
        {
            bool canBeLeader = _leaderElectionService.ReceiveLeaderRequest(node);
            return new ObjectResult(new NodeVote { VoteValue = canBeLeader });
        }
    }
}