using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities.List;
using Neutrino.Entities.Model;

namespace Neutrino.Core.Services
{
    public interface IServiceHealthService
    {
        IEnumerable<ServiceHealth> Get();

        PageList<ServiceHealth> Get(string serviceId, int offset = 0, int limit = Int32.MaxValue);

        ServiceHealth GetCurrent(string serviceId);

        ServiceHealth Get(string serviceId, string id);

        void Create(string serviceId, ServiceHealth serviceHealth);

        void Clear();
    }
}