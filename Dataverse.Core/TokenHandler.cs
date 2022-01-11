using System.Net.Http.Headers;

namespace Dataverse.Core;

public class TokenHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;

    public TokenHandler(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenService.GetAccessToken());
        return await base.SendAsync(request, cancellationToken);
    }
}