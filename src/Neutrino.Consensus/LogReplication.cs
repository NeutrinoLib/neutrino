using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Microsoft.Extensions.Logging;
using Neutrino.Consensus.Entities;
using Neutrino.Consensus.Events;
using Neutrino.Consensus.Responses;
using Newtonsoft.Json;

namespace Neutrino.Consensus
{
    public class LogReplication : ILogReplication
    {
        private readonly IConsensusContext _consensusContext;
        private readonly HttpClient _httpClient;
        private readonly ILogger<LogReplication> _logger;

        public LogReplication(
            IConsensusContext consensusContext, 
            HttpClient httpClient,
            ILogger<LogReplication> logger)
        {
            _consensusContext = consensusContext;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ConsensusResult> DistributeEntry(object objectData, MethodType method)
        {
            var entries = new List<Entry>();
            var entry = new Entry
            {
                ObjectType = objectData.GetType().FullName,
                Value = objectData,
                Method = method
            };
            entries.Add(entry);

            var tasks = SendAppendEntries(entries);
            try
            {
                await Task.WhenAll(tasks.ToArray());
            }
            catch(Exception exception) 
            { 
                _logger.LogDebug("Exception during distributing entries.", exception);
            }
            
            int successful = await CollectResponses(tasks);

            var allNodes = _consensusContext.NodeStates.Count + 1;
            if ((successful * 2) >= allNodes)
            {
                return ConsensusResult.CreateSuccessful();
            }

            return ConsensusResult.CreateError("Log wasn't replicated to majority amount of the nodes.");
        }

        private static async Task<int> CollectResponses(List<Task<HttpResponseMessage>> tasks)
        {
            int successful = 1;
            foreach (var task in tasks)
            {
                if (task.Status == TaskStatus.RanToCompletion && task.Result.IsSuccessStatusCode)
                {
                    var result = await task.Result.Content.ReadAsStringAsync();

                    var appendEntriesResponse = JsonConvert.DeserializeObject<AppendEntriesResponse>(result);
                    if (appendEntriesResponse.WasSuccessful)
                    {
                        successful++;
                    }
                }
            }

            return successful;
        }

        private List<Task<HttpResponseMessage>> SendAppendEntries(List<Entry> entries)
        {
            var tasks = new List<Task<HttpResponseMessage>>();
            try
            {
                foreach (var node in _consensusContext.NodeStates)
                {
                    var task = SendAppendEntries(node.NodeAddress, entries);
                    if(task != null)
                    {
                        tasks.Add(task);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogDebug("Exception during collecting tasks to distribute entries.", exception);
            }

            return tasks;
        }

        private Task<HttpResponseMessage> SendAppendEntries(string nodeAddress, IList<Entry> entries)
        {
            try
            {
                var url = nodeAddress.AppendPathSegment("api/raft/append-entries");
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    _consensusContext.ConsensusOptions.AuthenticationScheme, _consensusContext.ConsensusOptions.AuthenticationParameter);

                var appendEntriesEvent = new AppendEntriesEvent(_consensusContext.CurrentTerm, _consensusContext.CurrentNode, entries);
                var jsonContent = JsonConvert.SerializeObject(appendEntriesEvent);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                request.Content = content;

                var task = _httpClient.SendAsync(request);
                return task;
            }
            catch (Exception exception)
            {
                _logger.LogDebug("Exception during sending entries.", exception);
                return null;
            }
        }
    }
}