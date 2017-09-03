using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Neutrino.Core.Services.Parameters;

namespace Neutrino.Api.Authentication
{
    /// <summary>
    /// Secure token authentication handler.
    /// </summary>
    public class SecureTokenAuthenticationHandler : IAuthenticationHandler
    {
        private HttpContext _context;
        private AuthenticationScheme _scheme;

        private readonly ApplicationParameters _applicationParameters;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="applicationParameters">Application parameters.</param>
        public SecureTokenAuthenticationHandler(IOptions<ApplicationParameters> applicationParameters)
        {
            _applicationParameters = applicationParameters.Value;
        }

        /// <summary>
        /// Authentication behavior.
        /// </summary>
        /// <returns>The AuthenticateResult result</returns>
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            string authorization = _context.Request.Headers["Authorization"];
            string token = null;

            if (string.IsNullOrWhiteSpace(authorization))
            {
                return Task.FromResult(AuthenticateResult.Fail($"Authorization header not exists."));
            }

            if (authorization.StartsWith($"{_scheme.Name} ", StringComparison.OrdinalIgnoreCase))
            {
                token = authorization.Substring($"{_scheme.Name} ".Length).Trim();
            }
            else
            {
                //return Task.FromResult(AuthenticateResult.NoResult());
                return Task.FromResult(AuthenticateResult.Fail($"Header for {_scheme.Name} scheme not exists."));
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return Task.FromResult(AuthenticateResult.Fail($"Value for {_scheme.Name} scheme not exists."));
            }

            bool isValid = ValidateToken(token);
            if (isValid)
            {
                var identity = new ClaimsIdentity(_scheme.Name, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "SystemId"));
                identity.AddClaim(new Claim(ClaimTypes.Name, "System"));

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), _scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail($"{_scheme.Name} is invalid."));
        }

        /// <summary>
        /// Challenge behavior.
        /// </summary>
        /// <param name="properties">The AuthenticationProperties that contains the extra meta-data arriving with the authentication.</param>
        /// <returns>A task.</returns>
        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            _context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            _context.Response.Headers.Append(HeaderNames.WWWAuthenticate, $"SecureToken realm=neutrino-api");
            return Task.FromResult(0);
        }

        /// <summary>
        /// Forbid behavior.
        /// </summary>
        /// <param name="properties">The AuthenticationProperties that contains the extra meta-data arriving with the authentication.</param>
        /// <returns>A task.</returns>
        public Task ForbidAsync(AuthenticationProperties properties)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// The handler should initialize anything it needs from the request and scheme here.
        /// </summary>
        /// <param name="scheme">The AuthenticationScheme scheme.</param>
        /// <param name="context">The HttpContext context.</param>
        /// <returns>A task.</returns>
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _scheme = scheme;
            _context = context;

            return Task.FromResult(0);
        }

        private bool ValidateToken(string token)
        {
            return _applicationParameters.SecureToken.Equals(token, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}