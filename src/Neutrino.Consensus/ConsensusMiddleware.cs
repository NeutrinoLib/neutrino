using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Neutrino.Consensus.Events;
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
                using (var bodyReader = new StreamReader(context.Request.Body))
                {
                    string body = await bodyReader.ReadToEndAsync();
                    var entity = JsonConvert.DeserializeObject<AppendEntriesEvent>(body);

                    var response = consensusContext.TriggerEvent(entity);
                    context.Response.StatusCode = (int) HttpStatusCode.OK;
                }
            }
            else if (string.Equals(path, "/api/raft/request-vote", StringComparison.CurrentCultureIgnoreCase))
            {
                using (var bodyReader = new StreamReader(context.Request.Body))
                {
                    string body = await bodyReader.ReadToEndAsync();
                    var entity = JsonConvert.DeserializeObject<RequestVoteEvent>(body);

                    var response = consensusContext.TriggerEvent(entity);
                    var responseString = JsonConvert.SerializeObject(response);

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int) HttpStatusCode.OK;
                    await context.Response.WriteAsync(responseString);
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}