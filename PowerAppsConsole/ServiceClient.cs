using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PowerAppsConsole
{
    public class ServiceClient
    {
        public ServiceClient(PowerAppClientSettings powerAppClientSettings)
        {
            PowerAppClientSettings = powerAppClientSettings;
        }

        const string DataverseUrl = "https://org55054fae.api.crm.dynamics.com";
        const string ClientId = "949414b1-0e94-42f0-8347-f7b21c95dccd";
        const string ClientSecret = "dkk7Q~EcMtBkKH1IsDHUGSEOP3R_oALDy.oFg";
        const string TenantId = "d1a3d763-f8b9-408b-b481-abacff2764c6";

        public PowerAppClientSettings PowerAppClientSettings { get; }

        /// <summary>
        /// Get Token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetToken()
        {
            var authContext = new AuthenticationContext($"https://login.microsoftonline.com/{TenantId}", false);
            var cred = new ClientCredential(PowerAppClientSettings.ClientId, PowerAppClientSettings.ClientSecret);
            var token = await authContext.AcquireTokenAsync(PowerAppClientSettings.DataverseUrl, cred);

            Console.WriteLine(token.AccessToken);

            return token.AccessToken;
        }

        public async Task<HttpClient> CreateClient()
        {
            var token = await GetToken();
            return CreateClient(token);
        }

        /// <summary>
        /// Create Http Client
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public HttpClient CreateClient(string token)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(PowerAppClientSettings.DataverseUrl + "/api/data/v9.2/"),
                Timeout = new TimeSpan(0, 2, 0)    // Standard two minute timeout on web service calls.
            };

            HttpRequestHeaders headers = client.DefaultRequestHeaders;
            headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");
            headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        /// <summary>
        /// Verify Endpoint Working
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task Verify(HttpClient client)
        {
            var response = await client.GetAsync("WhoAmI");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}
