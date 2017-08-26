using System.Net.Http;

namespace Neutrino.Client
{
    public interface IHttpRequestService
    {
        HttpClient GetHttpClient();
    }
}