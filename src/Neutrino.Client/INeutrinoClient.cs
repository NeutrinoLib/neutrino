using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities;

namespace Neutrino.Client
{
    public interface INeutrinoClient
    {
        Task<IList<Service>> GetServicesAsync();

        Task<IList<Service>> GetServicesByServiceTypeAsync(string serviceType);

        Task<IList<Service>> GetServicesByTagsAsync(params string[] tags);

        Task<Service> GetServiceByIdAsync(string id);

        Task<Service> GetServiceByAddressAsync(string address);

        Task<ActionConfirmation<Service>> AddServiceAsync(Service service);

        Task<ActionConfirmation> UpdateServiceAsync(string id, Service service);

        Task<ActionConfirmation> DeleteServiceAsync(string id);

        Task<KvProperty> GetKvPropertyAsync(string key);

        Task<ActionConfirmation<KvProperty>> AddKvPropertyAsync(KvProperty kvProperty);

        Task<ActionConfirmation> UpdateKvPropertyAsync(string key, KvProperty kvProperty);

        Task<ActionConfirmation> DeleteKvPropertyAsync(string key);

        Task<IList<ServiceHealth>> GetServiceHealthAsync(string serviceId);

        Task<PageList<ServiceHealth>> GetServiceHealthAsync(string serviceId, int offset, int limit);

        Task<ServiceHealth> GetLastServiceHealthAsync(string serviceId);
    }
}