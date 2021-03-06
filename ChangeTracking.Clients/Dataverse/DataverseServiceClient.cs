using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChangeTracking.Core.Configuration;

namespace ChangeTracking.Clients.Dataverse
{
    public class DataverseServiceClient
    {
        private HttpClient _client;

        public DataverseServiceClient(HttpClient client, PowerAppTokenSettings settings)
        {
            _client = client;
            InitClient(settings);
        }

        public HttpClient Client => _client;
        private void InitClient(PowerAppTokenSettings settings)
        {
            AddDefaultHeaders();
        }

        private void AddDefaultHeaders()
        {
            _client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            _client.DefaultRequestHeaders.Add("OData-Version", "4.0");
        }

        public void AddHeaders(Action<HttpRequestHeaders> headerAction)
        {
            _client.DefaultRequestHeaders.Clear();
            headerAction(_client.DefaultRequestHeaders);
            AddDefaultHeaders();
        }

        /// <summary>
        /// Verify Endpoint Working
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public async Task<bool> Verify()
        {
            var response = await _client.GetAsync("WhoAmI");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}