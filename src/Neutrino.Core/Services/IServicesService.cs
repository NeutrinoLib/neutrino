using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public interface IServicesService
    {
        IEnumerable<Service> Get();

        Service Get(string id);

        ActionConfirmation Create(Service service);

        ActionConfirmation Update(Service service);

        void Delete(string id);

        void RunHealthChecker();

        void StopHealthChecker();
    }
}