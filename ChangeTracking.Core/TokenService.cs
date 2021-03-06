using System.Threading.Tasks;
using ChangeTracking.Core.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ChangeTracking.Core
{
    /// <summary>
    /// Token Service
    /// </summary>
    public class TokenService
    {
        private readonly AuthenticationContext _context;
        public TokenService(AzureTokenSettings settings)
        {
            Settings = settings;
            _context = new AuthenticationContext($"https://login.microsoftonline.com/{Settings.TenantId}", false);
        }

        public AzureTokenSettings Settings { get; }

        /// <summary>
        /// Get Token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAccessToken()
        {
            var cred = new ClientCredential(Settings.ClientId, Settings.ClientSecret);
            var token = await _context.AcquireTokenAsync(Settings.ResourceUrl, cred);

            return token.AccessToken;
        }
    }
}
