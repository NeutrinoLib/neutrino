using System.Net.Http;
using System.Threading.Tasks;
using Neutrino.Entities.Response;

namespace Neutrino.Client
{
    public interface IHttpRequestService
    {
        Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path) where T: class;

        Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path, string id) where T: class;

        Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path, string id, string query) where T: class;

        Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path, string id, T objectData) where T: class;

        Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path, T objectData) where T: class;

        Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path, string id, string query, T objectData) where T: class;

        Task<ActionConfirmation> SendRequest(HttpMethod httpMethod, string path, string id = null);
    }
}