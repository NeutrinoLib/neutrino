using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Net.Http.Headers;

namespace Neutrino.Api.SecureTokenAuthentication
{
    /// <summary>
    /// Secure token handler.
    /// </summary>
    public class SecureTokenHandler : AuthenticationHandler<SecureTokenOptions>
    {
        /// <summary>
        /// Method handkle authentication based on secure token.
        /// </summary>
        /// <returns>Result of authentication.</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorization = Request.Headers["Authorization"];
            string token = null;

            if (string.IsNullOrWhiteSpace(authorization))
            {
                return await Task.FromResult(AuthenticateResult.Skip());
            }

            if (authorization.StartsWith($"{Options.AuthenticationScheme} ", StringComparison.OrdinalIgnoreCase))
            {
                token = authorization.Substring($"{Options.AuthenticationScheme} ".Length).Trim();
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return AuthenticateResult.Skip();
            }

            bool isValid = ValidateToken(token);
            if (isValid)
            {
                var identity = new ClaimsIdentity(Options.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "SystemId"));
                identity.AddClaim(new Claim(ClaimTypes.Name, "System"));

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Options.AuthenticationScheme);

                return await Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return AuthenticateResult.Fail($"{Options.AuthenticationScheme} is invalid.");
        }

        /// <summary>
        /// Method handle unathorization event.
        /// </summary>
        /// <param name="context">Context of challenge.</param>
        /// <returns>Returns result.</returns>
        protected override async Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            var authResult = await HandleAuthenticateOnceSafeAsync();

            if (!authResult.Skipped)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
            }

            Response.Headers.Append(HeaderNames.WWWAuthenticate, $"{Options.AuthenticationScheme} realm=\"{Options.Realm}\"");
            return false;
        }

        private bool ValidateToken(string token)
        {
            return Options.SecureToken.Equals(token, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}