using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

        public LogReplication(IConsensusContext consensusContext, HttpClient httpClient)
        {
            _consensusContext = consensusContext;
            _httpClient = httpClient;
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
            
            int successful = 1;
            foreach(var node in _consensusContext.NodeStates)
            {
                try
                {
                    var httpResponseMessage = await SendAppendEntries(node.Node, entries);
                    var result = await httpResponseMessage.Content.ReadAsStringAsync();

                    var appendEntriesResponse = JsonConvert.DeserializeObject<AppendEntriesResponse>(result);
                    if(appendEntriesResponse.WasSuccessful)
                    {
                        successful++;
                    }         
                }
                catch(Exception)
                {
                }
            }

            var allNodes = _consensusContext.NodeStates.Count + 1;
            if((successful * 2) >= allNodes) 
            {
                return ConsensusResult.CreateSuccessful();
            }
        
            return ConsensusResult.CreateError("Log wasn't replicated to majority amount of the nodes.");
        }

        private Task<HttpResponseMessage> SendAppendEntries(NodeInfo node, IList<Entry> entries)
        {
            var url = Path.Combine(node.Address, "api/raft/append-entries");
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var appendEntriesEvent = new AppendEntriesEvent(_consensusContext.CurrentTerm, _consensusContext.CurrentNode, entries);
            var jsonContent = JsonConvert.SerializeObject(appendEntriesEvent);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;

            var task = _httpClient.SendAsync(request);
            return task;
        }
    }
}