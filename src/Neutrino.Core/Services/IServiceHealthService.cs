using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public interface IServiceHealthService
    {
        IEnumerable<ServiceHealth> Get();

        IEnumerable<ServiceHealth> Get(string serviceId);

        ServiceHealth GetCurrent(string serviceId);

        ServiceHealth Get(string serviceId, string id);

        void Create(string serviceId, ServiceHealth serviceHealth);

        void Clear();
    }
}