using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerAppCore.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PowerAppCore
{
    class Program
    {
        static string ApiUrl = "https://org55054fae.api.crm.dynamics.com";
        static string clientId = "949414b1-0e94-42f0-8347-f7b21c95dccd";

        static async Task<int> Main()
        {
            var token = await GetToken();
            var client = CreateClient(token);

            //await InsertAccountRecord(client);
            await ListRecords(client);

            Console.ReadKey();

            return 1;
        }

        public static async Task<string> GetToken()
        {
            // The authentication context used to acquire the web service access token
            var authContext = new AuthenticationContext(
                                       "https://login.microsoftonline.com/d1a3d763-f8b9-408b-b481-abacff2764c6", false);
            //"https://login.microsoftonline.com/common", false);

            var cred = new ClientCredential(clientId, "dkk7Q~EcMtBkKH1IsDHUGSEOP3R_oALDy.oFg");

            var token = await authContext.AcquireTokenAsync(ApiUrl, cred);

            return token.AccessToken;
        }

        public static HttpClient CreateClient(string token)
        {
            var client = new HttpClient
            {
                // See https://docs.microsoft.com/en-us/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors#web-api-url-and-versions
                BaseAddress = new Uri(ApiUrl + "/api/data/v9.2/"),
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

        public static async Task WhoAmI(HttpClient client)
        {
            var response = await client.GetAsync("WhoAmI");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        public static async Task UpdateAccountRecord(HttpClient client)
        {
            var model = new AccountModel()
            {
                AccountNo = "13456798",
                Name = "Patch Console Record 2",
                Telephone1 = "0567432"
            };

            var accountId = "426b4262-fd6d-ec11-8943-000d3a37305a";
            var result = await client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), $"accounts({accountId})"));

            if (!HasFailure(result))
            {
                Console.WriteLine(await result.Content.ReadAsStringAsync());
            }
        }

        public static async Task InsertAccountRecord(HttpClient client)
        {
            var model = new AccountModel()
            {
                AccountNo = "234",
                Name = "Console Record 1",
                Telephone1 = "123456789"
            };

            var result = await client.PostAsync("accounts", new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json"));
            if (!HasFailure(result))
            {
                Console.WriteLine(await result.Content.ReadAsStringAsync());
            }
        }

        public static async Task ListRecords(HttpClient client)
        {
            var result = await client.GetAsync("accounts");
            if (!HasFailure(result))
            {
                Console.WriteLine(await result.Content.ReadAsStringAsync());
            }
        }

        public static bool HasFailure(HttpResponseMessage msg)
        {
            if (!msg.IsSuccessStatusCode)
            {
                Console.WriteLine("Error");
                Console.WriteLine(msg.Content.ReadAsStringAsync().Result);
                return true;
            }

            return false;
        }
    }
}