using System;
using System.Threading.Tasks;
using Neutrino.Consensus;
using Neutrino.Consensus.Entities;
using Neutrino.Consensus.Events;
using Neutrino.Entities;
using Newtonsoft.Json;

namespace Neutrino.Core.Services
{
    public class LogReplicationService : ILogReplicable
    {
        private readonly IServiceHealthService _serviceHealthService;
        private readonly IServicesService _servicesService;
        private readonly IKvPropertyService _kvPropertyService;

        public LogReplicationService(
            IServiceHealthService serviceHealthService, 
            IServicesService servicesService,
            IKvPropertyService kvPropertyService)
        {
            _serviceHealthService = serviceHealthService;
            _servicesService = servicesService;
            _kvPropertyService = kvPropertyService;
        }

        public bool OnLogReplication(AppendEntriesEvent appendEntriesEvent)
        {
            if(appendEntriesEvent.Entries != null && appendEntriesEvent.Entries.Count > 0)
            {
                foreach(var item in appendEntriesEvent.Entries)
                {
                    if(item.ObjectType == typeof(ServiceHealth).FullName)
                    {
                        var serviceHealth = JsonConvert.DeserializeObject<ServiceHealth>(item.Value.ToString());
                        _serviceHealthService.Create(serviceHealth.ServiceId, serviceHealth);
                    }
                    else if(item.ObjectType == typeof(Service).FullName)
                    {
                        var serviceData = JsonConvert.DeserializeObject<Service>(item.Value.ToString());
                        switch(item.Method)
                        {
                            case MethodType.Create:
                                _servicesService.Create(serviceData);
                                break;
                            case MethodType.Update:
                                _servicesService.Update(serviceData);
                                break;
                            case MethodType.Delete:
                                _servicesService.Delete(serviceData.Id);
                                break;
                        }
                    }
                    else if(item.ObjectType == typeof(KvProperty).FullName)
                    {
                        var kvProperty = JsonConvert.DeserializeObject<KvProperty>(item.Value.ToString());
                        _kvPropertyService.Create(kvProperty);
                    }
                }
            }

            return true;
        }
    }
}