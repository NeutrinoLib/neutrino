using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities;

namespace Neutrino.Core.Services
{
    public interface IServicesService
    {
        IEnumerable<Service> Get();

        Service Get(string id);

        void Create(Service service);

        void Update(string id, Service service);

        void Delete(string id);

        void RunHealthChecker();
    }
}