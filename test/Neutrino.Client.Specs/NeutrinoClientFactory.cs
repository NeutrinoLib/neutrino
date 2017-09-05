using System.IO;
using Microsoft.Extensions.Configuration;

namespace Neutrino.Client.Specs
{
    public static class NeutrinoClientFactory
    {
        public static NeutrinoClient GetNeutrinoClient()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            var serverAddress = configuration["ClientAddress"];

            var neutrinoClientOptions = new NeutrinoClientOptions();
            neutrinoClientOptions.Addresses = new string[] { serverAddress };
			neutrinoClientOptions.SecureToken = configuration["SecureToken"];

			var httpRequestService = new HttpRequestService(neutrinoClientOptions);
			return new NeutrinoClient(httpRequestService);
        }
    }
}