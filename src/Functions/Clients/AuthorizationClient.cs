using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Functions.Clients.Interfaces;
using Altinn.Platform.Authorization.Functions.Configuration;
using Altinn.Platform.Authorization.Functions.Models;
using Altinn.Platform.Authorization.Functions.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Altinn.Platform.Authorization.Functions.Clients;

/// <summary>
/// Client configuration for Authorization API
/// </summary>
public class AuthorizationClient : IAuthorizationClient
{
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly ILogger<AuthorizationClient> _logger;

    /// <summary>
    /// Gets an instance of httpclient from httpclientfactory
    /// </summary>
    public HttpClient Client { get; }

    /// <summary>
    /// Initializes the http client for access Authorization API
    /// </summary>
    /// <param name="client">the http client</param>
    /// <param name="accessTokenProvider">The provider giving an access token to use against the authorization API</param>
    /// <param name="platformSettings">the platform settings configured for the authorization functions</param>
    /// <param name="logger">The logger</param>
    public AuthorizationClient(HttpClient client, IAccessTokenProvider accessTokenProvider, IOptions<PlatformSettings> platformSettings, ILogger<AuthorizationClient> logger)
    {
        _accessTokenProvider = accessTokenProvider;
        _logger = logger;
        PlatformSettings settings = platformSettings.Value;
        Client = client;
        Client.BaseAddress = new Uri(settings.AuthorizationApiEndpoint);
        Client.Timeout = new TimeSpan(0, 0, 30);
        Client.DefaultRequestHeaders.Clear();
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Performs a POST request to the authorization API with the range of delegation events to replay
    /// </summary>
    /// <param name="startId">The first id in the range to replay</param>
    /// <param name="endId">The last id in the range to replay. If left to 0 all events after the startId will be replayed</param>
    /// <returns>A HTTP response message</returns>
    public async Task<HttpResponseMessage> PostDelegationEventsReplayAsync(int startId, int endId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"delegations/delegationchangeevents/replay?startId={startId}&endId={endId}")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", await _accessTokenProvider.GetAccessToken())
            }
        };

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "AuthorizationClient posting eventIds for replay {url} with token: {token}, startId: {startId}, endId: {endId}",
                request.RequestUri,
                request.Headers.Authorization.ToString(),
                startId,
                endId);
        }

        return await Client.SendAsync(request);
    }
}
