using System;
using System.Collections.Generic;
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
                        switch(item.Method)
                        {
                            case MethodType.Create:
                                _kvPropertyService.Create(kvProperty);
                                break;
                            case MethodType.Update:
                                _kvPropertyService.Update(kvProperty);
                                break;
                            case MethodType.Delete:
                                _kvPropertyService.Delete(kvProperty.Key);
                                break;
                        }
                    }
                }
            }

            return true;
        }

        public bool ClearLog()
        {
            _kvPropertyService.Clear();
            _serviceHealthService.Clear();
            _servicesService.Clear();

            return true;
        }

        public AppendEntriesEvent GetFullLog()
        {
            var appendEntriesEvent = new AppendEntriesEvent();
            appendEntriesEvent.Entries = new List<Entry>();

            var services = _servicesService.Get();
            var kvProperties = _kvPropertyService.Get();
            var serviceHealth = _serviceHealthService.Get();

            AppendItems(appendEntriesEvent, services);
            AppendItems(appendEntriesEvent, kvProperties);
            AppendItems(appendEntriesEvent, serviceHealth);

            return appendEntriesEvent;
        }

        private static void AppendItems(AppendEntriesEvent appendEntriesEvent, IEnumerable<dynamic> services)
        {
            foreach (var service in services)
            {
                var entry = new Entry
                {
                    Method = MethodType.Create,
                    Value = service,
                    ObjectType = service.GetType().FullName
                };

                appendEntriesEvent.Entries.Add(entry);
            }
        }
    }
}