using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace ChangeTracking.Core.Store;

public class BlobTokenStore : IPowerAppTokenStore
{
    private readonly BlobContainerClient _containerClient;

    public BlobTokenStore(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    public async Task Store(string keyName, string token)
    {
        var blobClient = _containerClient.GetBlobClient(keyName);
        await blobClient.UploadAsync(new BinaryData(token));
    }

    public async Task<string> Get(string keyName)
    {
        var blobClient = _containerClient.GetBlobClient(keyName);
        if (!await blobClient.ExistsAsync())
            return string.Empty;

        var content = await blobClient.DownloadContentAsync();
        return content.Value.Content.ToString();
    }
}
public class InMemoryTokenStore : IPowerAppTokenStore
{
    private Dictionary<string, string> ChangeToken { get; set; }
    public Task Store(string keyName, string token)
    {
        if (!ChangeToken.ContainsKey(keyName))
        {
            ChangeToken.Add(keyName, token);
        }
        else
        {
            ChangeToken[keyName] = token;
        }

        return Task.CompletedTask;
    }

    public Task<string> Get(string keyName)
    {
        if (ChangeToken.ContainsKey(keyName))
        {
            return Task.FromResult(string.Empty);
        }

        return Task.FromResult(ChangeToken[keyName]);
    }
}