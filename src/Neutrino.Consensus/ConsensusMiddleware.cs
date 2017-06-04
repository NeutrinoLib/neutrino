using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Neutrino.Consensus.Events;
using Neutrino.Consensus.Responses;
using Newtonsoft.Json;

namespace Neutrino.Consensus
{
    public class ConsensusMiddleware
    {
        private readonly RequestDelegate _next;

        public ConsensusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IConsensusContext consensusContext)
        {
            var path = context.Request.Path.ToString();
            if (string.Equals(path, "/api/raft/append-entries", StringComparison.CurrentCultureIgnoreCase))
            {
                await InvokeAppendEntries(context, consensusContext);
            }
            else if (string.Equals(path, "/api/raft/request-vote", StringComparison.CurrentCultureIgnoreCase))
            {
                await InvokeRequestVote(context, consensusContext);
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private static async Task InvokeRequestVote(HttpContext context, IConsensusContext consensusContext)
        {
            using (var bodyReader = new StreamReader(context.Request.Body))
            {
                string body = await bodyReader.ReadToEndAsync();
                var entity = JsonConvert.DeserializeObject<RequestVoteEvent>(body);

                var response = consensusContext.State.TriggerEvent(entity);
                var responseString = JsonConvert.SerializeObject(response);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync(responseString);
            }
        }

        private static async Task InvokeAppendEntries(HttpContext context, IConsensusContext consensusContext)
        {
            using (var bodyReader = new StreamReader(context.Request.Body))
            {
                string body = await bodyReader.ReadToEndAsync();
                var entity = JsonConvert.DeserializeObject<AppendEntriesEvent>(body);

                consensusContext.State.TriggerEvent(entity);

                IResponse response = null;
                if (entity.Entries != null && entity.Entries.Count > 0)
                {
                    var isSuccessfull = consensusContext.Options.OnLogReplicationCallback?.Invoke(entity);
                    response = new AppendEntriesResponse(consensusContext.CurrentTerm, isSuccessfull.HasValue ? isSuccessfull.Value : false);
                }
                else
                {
                    response = new AppendEntriesResponse(consensusContext.CurrentTerm, true);
                }

                var responseString = JsonConvert.SerializeObject(response);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync(responseString);
            }
        }
    }
}