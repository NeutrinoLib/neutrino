using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Neutrino.Api.Consensus.Events;
using Newtonsoft.Json;

namespace Neutrino.Api.Consensus
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
            // if specific condition does not meet
            if (context.Request.Path.ToString().Equals("/api/raft/heartbeat"))
            {
                using (var bodyReader = new StreamReader(context.Request.Body))
                {
                    string body = await bodyReader.ReadToEndAsync();
                    var entity = JsonConvert.DeserializeObject<HeartbeatEvent>(body);

                    var response = consensusContext.TriggerEvent(entity);
                    context.Response.StatusCode = (int) HttpStatusCode.OK;
                }
            }
            else if(context.Request.Path.ToString().Equals("/api/raft/leader"))
            {
                using (var bodyReader = new StreamReader(context.Request.Body))
                {
                    string body = await bodyReader.ReadToEndAsync();
                    var entity = JsonConvert.DeserializeObject<LeaderRequestEvent>(body);

                    var response = consensusContext.TriggerEvent(entity);
                    var responseString = JsonConvert.SerializeObject(response);

                    context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
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