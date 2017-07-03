using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Neutrino.Api.SecureTokenAuthentication
{
    /// <summary>
    /// Secure token middleware.
    /// </summary>
    public class SecureTokenMiddleware : AuthenticationMiddleware<SecureTokenOptions>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="next">Next delegate.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        /// <param name="encoder">Url encoder.</param>
        /// <param name="options">Secure token options.</param>
        public SecureTokenMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, UrlEncoder encoder, IOptions<SecureTokenOptions> options)
            : base(next, options, loggerFactory, encoder)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
        }

        /// <summary>
        /// Creates handler for secure token authentication.
        /// </summary>
        /// <returns>New handler.</returns>
        protected override AuthenticationHandler<SecureTokenOptions> CreateHandler()
        {
            return new SecureTokenHandler();
        }
    }
}