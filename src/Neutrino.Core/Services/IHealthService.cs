using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public interface IHealthService
    {
        void RunHealthChecker(Service service);

        void StopHealthChecker(string serviceId);

        ServiceHealth GetServiceHealth(string serviceId);
    }
}