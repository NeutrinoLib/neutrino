using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Neutrino.AspNetCore.Client
{
    public static class NeutrinoClientExtensions
    {
        public static IServiceCollection AddNeutrinoClient(this IServiceCollection services, Action<NeutrinoClientOptions> optionsAction)
        {
            var options = new NeutrinoClientOptions();
            optionsAction?.Invoke(options);
            services.AddSingleton<INeutrinoClientOptions>(options);

            services.AddSingleton<IHttpRequestService, HttpRequestService>();
            services.AddScoped<INeutrinoClient, NeutrinoClient>();
            return services;
        }
    }
}