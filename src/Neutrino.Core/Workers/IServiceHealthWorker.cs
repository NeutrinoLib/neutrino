using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities.Model;

namespace Neutrino.Core.Workers
{
    public interface IServiceHealthWorker
    {
        void RunHealthChecker(Service service);

        void StopHealthChecker(string serviceId);

        ServiceHealth GetServiceHealth(string serviceId);
    }
}