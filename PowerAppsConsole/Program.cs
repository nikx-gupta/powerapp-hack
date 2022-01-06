using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerAppsConsole.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebAPIQuickStart
{
    class Program
    {
        static string ApiUrl = "https://org55054fae.api.crm.dynamics.com";
        static string clientId = "949414b1-0e94-42f0-8347-f7b21c95dccd";

        static void Main()
        {
            var token = GetToken();
            var client = CreateClient(token);
            InsertAccountRecord(client);

            Console.ReadKey();
        }

        public static string GetToken()
        {
            // The authentication context used to acquire the web service access token
            var authContext = new AuthenticationContext(
                                       "https://login.microsoftonline.com/d1a3d763-f8b9-408b-b481-abacff2764c6", false);
            //"https://login.microsoftonline.com/common", false);

            var cred = new ClientCredential(clientId, "dkk7Q~EcMtBkKH1IsDHUGSEOP3R_oALDy.oFg");

            var token = authContext.AcquireTokenAsync(ApiUrl, cred).Result;

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

        public static void WhoAmI(HttpClient client)
        {
            var response = client.GetAsync("WhoAmI").Result;
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        }

        public static void InsertAccountRecord(HttpClient client)
        {
            var model = new AccountModel()
            {
                AccountNo = "234",
                Name = "Console Record 1",
                Telephone1 = "123456789"
            };

            var result = client.PostAsync("accounts", new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json")).Result;
            if (!HandleFailure(result))
            {
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            }
        }

        public static bool HandleFailure(HttpResponseMessage msg)
        {
            if (!msg.IsSuccessStatusCode)
            {
                Console.WriteLine("Error");
                Console.WriteLine(msg.Content.ReadAsStringAsync().Result);
                return false;
            }

            return true;
        }
    }
}