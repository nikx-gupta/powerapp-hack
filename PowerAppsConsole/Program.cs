using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerAppsConsole.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PowerAppsConsole
{
    class Program
    {
        const string DataverseUrl = "https://org55054fae.api.crm.dynamics.com";
        const string ClientId = "949414b1-0e94-42f0-8347-f7b21c95dccd";
        const string ClientSecret = "dkk7Q~EcMtBkKH1IsDHUGSEOP3R_oALDy.oFg";
        const string TenantId = "d1a3d763-f8b9-408b-b481-abacff2764c6";

        static async Task<int> Main()
        {
            var token = await GetToken();
            var client = CreateClient(token);
            var crudRepo = new CrudOperation<AccountModel>(client, "accounts");


            var rec = await crudRepo.Insert(new AccountModel()
            {
                AccountNo = Guid.NewGuid().ToString().Substring(0, 20),
                Name = DateTime.Now.ToString("dd-MM-yyyy HH:MM"),
                Telephone1 = "123456789"
            });

            string accountId = rec.AccountId;
           
            await crudRepo.UpdateRecord(accountId, new AccountModel()
            {
                AccountNo = Guid.NewGuid().ToString().Substring(0, 20),
                Name = DateTime.Now.ToString("dd-MM-yyyy HH:MM"),
                Telephone1 = "123456789"
            });

            await crudRepo.UpdateSingleProperty(accountId, "name", DateTime.Now.ToString("dd-MM-yyyy HH:MM"));

            await crudRepo.ListRecords();

            await crudRepo.GetRecord(accountId);

            await crudRepo.DeleteProperty(accountId, "name");

            await crudRepo.Delete(accountId);

            Console.ReadKey();

            return 1;
        }

        /// <summary>
        /// Get Token
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetToken()
        {
            var authContext = new AuthenticationContext($"https://login.microsoftonline.com/{TenantId}", false);
            var cred = new ClientCredential(ClientId, ClientSecret);
            var token = await authContext.AcquireTokenAsync(DataverseUrl, cred);

            Console.WriteLine(token.AccessToken);

            return token.AccessToken;
        }

        /// <summary>
        /// Create Http Client
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static HttpClient CreateClient(string token)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(DataverseUrl + "/api/data/v9.2/"),
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
        public static async Task WhoAmI(HttpClient client)
        {
            var response = await client.GetAsync("WhoAmI");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}