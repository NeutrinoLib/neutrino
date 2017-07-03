using Microsoft.AspNetCore.Builder;

namespace Neutrino.Api.SecureTokenAuthentication
{
    /// <summary>
    /// Secure token options.
    /// </summary>
    public class SecureTokenOptions : AuthenticationOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SecureTokenOptions() : base()
        {
            AuthenticationScheme = SecureTokenDefaults.AuthenticationScheme;
            AutomaticAuthenticate = true;
            AutomaticChallenge = true;
        }

        /// <summary>
        /// Secure token.
        /// </summary>
        public string SecureToken { get; set; } 

        /// <summary>
        /// Realm.
        /// </summary>
        /// <returns></returns>
        public string Realm { get; set; }
    }
}