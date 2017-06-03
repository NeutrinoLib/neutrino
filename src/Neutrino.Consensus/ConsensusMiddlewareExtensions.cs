using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Neutrino.Api.Consensus
{
    public static class ConsensusMiddlewareExtensions
    {
        public static IApplicationBuilder UseConsensus(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ConsensusMiddleware>();
        }

        public static IServiceCollection AddConsensus(this IServiceCollection services)
        {
            return services.AddSingleton<IConsensusContext, ConsensusContext>();
        }
    }
}