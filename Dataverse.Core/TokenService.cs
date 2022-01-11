using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dataverse.Core.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Dataverse.Core
{
    /// <summary>
    /// Token Service
    /// </summary>
    public class TokenService
    {
        private readonly AuthenticationContext _context;
        public TokenService(PowerAppClientSettings settings)
        {
            Settings = settings;
            _context = new AuthenticationContext($"https://login.microsoftonline.com/{Settings.TenantId}", false);
        }

        public PowerAppClientSettings Settings { get; }

        /// <summary>
        /// Get Token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAccessToken()
        {
            var cred = new ClientCredential(Settings.ClientId, Settings.ClientSecret);
            var token = await _context.AcquireTokenAsync(Settings.DataverseUrl, cred);

            Console.WriteLine(token.ExpiresOn);

            return token.AccessToken;
        }
    }
}
