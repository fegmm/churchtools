using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using static Microsoft.Kiota.Abstractions.Authentication.ApiKeyAuthenticationProvider;

namespace Fegmm.ChurchTools;

/// <summary>
/// Factory for creating <see cref="ChurchToolsClient"/> instances using an injected <see cref="HttpClient"/>.
/// </summary>
public class ChurchToolsClientFactory(HttpClient httpClient, IOptions<ChurchToolsOptions> options)
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ChurchToolsOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));

    public ChurchToolsClient GetClient()
    {
        ApiKeyAuthenticationProvider authProvider = new(_options.ApiToken, "Authorization", KeyLocation.Header);
        HttpClientRequestAdapter httpClientRequestAdapter = new(authProvider, httpClient: _httpClient)
        {
            BaseUrl = _options.BaseUrl
        };
        return new ChurchToolsClient(httpClientRequestAdapter);
    }
}
