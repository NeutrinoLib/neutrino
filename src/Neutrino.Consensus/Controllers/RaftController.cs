using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neutrino.Consensus.Events;
using Neutrino.Consensus.Responses;
using Newtonsoft.Json;

namespace Neutrino.Consensus.Controllers
{
    /// <summary>
    /// Controller for Raft signals.
    /// </summary>
    [Authorize]
    [Route("api/raft")]
    public class RaftController : Controller
    {
        private readonly IConsensusContext _consensusContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="consensusContext">Consensus context.</param>
        public RaftController(IConsensusContext consensusContext)
        {
            _consensusContext = consensusContext;
        }

        /// <summary>
        /// Append log entries.
        /// </summary>
        /// <remarks>
        /// Endpoint for appending log entries. It's alos used for heartbeats.
        /// </remarks>
        /// <param name="appendEntriesEvent">New log data.</param>
        /// <returns>Returns result of method.</returns>
        [HttpPost("append-entries")]
        public ActionResult AppendEntries([FromBody] AppendEntriesEvent appendEntriesEvent)
        {
            IResponse response = null;
            if(_consensusContext.State == null)
            {
                response = new AppendEntriesResponse(_consensusContext.CurrentTerm, true);
                return new ObjectResult(response);
            }

            _consensusContext.State.TriggerEvent(appendEntriesEvent);
            if (appendEntriesEvent.Entries != null && appendEntriesEvent.Entries.Count > 0)
            {
                var isSuccessfull = _consensusContext.LogReplicable.OnLogReplication(appendEntriesEvent);
                response = new AppendEntriesResponse(_consensusContext.CurrentTerm, isSuccessfull);
            }
            else
            {
                response = new AppendEntriesResponse(_consensusContext.CurrentTerm, true);
            }

            return new ObjectResult(response);
        }

        /// <summary>
        /// Voting for new leader.
        /// </summary>
        /// <remarks>
        /// Endpoint is used during election new leader.
        /// </remarks>
        /// <param name="requestVoteEvent">Voting information.</param>
        /// <returns>Voting result.</returns>
        [HttpPost("request-vote")]
        public ActionResult RequestVote([FromBody] RequestVoteEvent requestVoteEvent)
        {
            var response = _consensusContext.State.TriggerEvent(requestVoteEvent);
            return new ObjectResult(response);
        }

        /// <summary>
        /// Returns full log.
        /// </summary>
        /// <remarks>
        /// Endpoint is used for retrieve full log from node.
        /// </remarks>
        /// <returns>Full log.</returns>
        [HttpGet("full-log")]
        public ActionResult FullLog()
        {
            var appendEntriesEvent = _consensusContext.LogReplicable.OnGetFullLog();
            return new ObjectResult(appendEntriesEvent);
        }
    }
}