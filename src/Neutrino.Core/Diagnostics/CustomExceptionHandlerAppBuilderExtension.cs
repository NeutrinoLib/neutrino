using Microsoft.AspNetCore.Builder;

namespace Neutrino.Core.Diagnostics
{
    public static class CustomExceptionHandlerAppBuilderExtension
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}