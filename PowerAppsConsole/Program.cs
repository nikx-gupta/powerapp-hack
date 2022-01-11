using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PowerAppsConsole
{
    //class Program
    //{
    //    const string DataverseUrl = "https://org55054fae.api.crm.dynamics.com";
    //    const string ClientId = "949414b1-0e94-42f0-8347-f7b21c95dccd";
    //    const string ClientSecret = "dkk7Q~EcMtBkKH1IsDHUGSEOP3R_oALDy.oFg";
    //    const string TenantId = "d1a3d763-f8b9-408b-b481-abacff2764c6";

    //    static async Task<int> Main()
    //    {
    //        var client = new ServiceClient(new PowerAppClientSettings()
    //        {
    //            DataverseUrl = DataverseUrl,
    //            ClientId = ClientId,
    //            ClientSecret = ClientSecret,
    //            TenantId = TenantId
    //        });

         
    //        return 1;
    //    }
    //}
}