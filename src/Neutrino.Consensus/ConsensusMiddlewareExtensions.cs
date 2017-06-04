using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neutrino.Consensus.Options;

namespace Neutrino.Consensus
{
    public static class ConsensusMiddlewareExtensions
    {
        public static IApplicationBuilder UseConsensus(this IApplicationBuilder builder, Action<ConsensusOptions> setupAction)
        {
            var appBuilder =  builder.UseMiddleware<ConsensusMiddleware>();

            var options = new ConsensusOptions();
            setupAction?.Invoke(options);

            var consensusContext = builder.ApplicationServices.GetService<IConsensusContext>();
            consensusContext.Run(options);

            return appBuilder;
        }

        public static IServiceCollection AddConsensus(this IServiceCollection services)
        {
            services.AddScoped<ILogReplication, LogReplication>();
            services.AddSingleton<HttpClient, HttpClient>();
            return services.AddSingleton<IConsensusContext, ConsensusContext>();
        }
    }
}