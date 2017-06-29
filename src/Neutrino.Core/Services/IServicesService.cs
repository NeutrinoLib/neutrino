using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public interface IServicesService
    {
        IEnumerable<Service> Get();

        Service Get(string id);

        Task<ActionConfirmation> Create(Service service);

        Task<ActionConfirmation> Update(Service service);

        Task<ActionConfirmation> Delete(string id);

        void RunHealthChecker();

        void StopHealthChecker();
    }
}