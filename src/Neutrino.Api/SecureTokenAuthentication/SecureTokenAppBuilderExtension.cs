using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Neutrino.Api.SecureTokenAuthentication
{
    /// <summary>
    /// Secure token extensions.
    /// </summary>
    public static class SecureTokenAppBuilderExtension
    {
        /// <summary>
        /// Use secure token extension method.
        /// </summary>
        /// <param name="applicationBuilder">Application builder.</param>
        /// <param name="options">Secure token options.</param>
        /// <returns>Application builder.</returns>
        public static IApplicationBuilder UseSecureTokenAuthentication(this IApplicationBuilder applicationBuilder, SecureTokenOptions options)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return applicationBuilder.UseMiddleware<SecureTokenMiddleware>(Options.Create(options));
        }
    }
}