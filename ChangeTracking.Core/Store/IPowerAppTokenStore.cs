using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs.Specialized;
using ChangeTracking.Core.Configuration;

namespace ChangeTracking.Core.Store
{
    public interface IPowerAppTokenStore
    {
        Task Store(string keyName, string token);
        Task<string> Get(string keyName);
    }
}
