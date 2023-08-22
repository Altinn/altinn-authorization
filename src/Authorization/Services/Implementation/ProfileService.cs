using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Common.AccessTokenClient.Services;
using Altinn.Platform.Authorization.Configuration;
using Altinn.Platform.Authorization.Helpers;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Profile.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Altinn.AccessManagement.Integration.Clients;

/// <summary>
/// A client for retrieving profiles from Altinn Platform.
/// </summary>
public class ProfileService
{
    private readonly ILogger _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PlatformSettings _settings;
    private readonly HttpClient _client;
    private readonly IAccessTokenGenerator _accessTokenGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileService"/> class
    /// </summary>
    /// <param name="platformSettings">the platform settings</param>
    /// <param name="logger">the logger</param>
    /// <param name="httpContextAccessor">The http context accessor </param>
    /// <param name="settings">The application settings.</param>
    /// <param name="httpClient">A HttpClient provided by the HttpClientFactory.</param>
    /// <param name="accessTokenGenerator">An instance of the AccessTokenGenerator service.</param>
    public ProfileService(
        IOptions<PlatformSettings> platformSettings,
        ILogger<ProfileService> logger,
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<PlatformSettings> settings,
        HttpClient httpClient,
        IAccessTokenGenerator accessTokenGenerator)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _settings = settings.CurrentValue;
        httpClient.BaseAddress = new Uri(platformSettings.Value.ApiProfileEndpoint);
        httpClient.DefaultRequestHeaders.Add(platformSettings.Value.SubscriptionKeyHeaderName, platformSettings.Value.SubscriptionKey);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client = httpClient;
        _accessTokenGenerator = accessTokenGenerator;
    }

    /// <summary>
    /// Method to get user profile
    /// </summary>
    /// <param name="userId">the userid</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<UserProfile> GetUserProfile(int userId)
    {
        UserProfile userProfile = null;

        //string endpointUrl = $"users/{userId}";
        //string token = JwtTokenUtil.GetTokenFromContext(_httpContextAccessor.HttpContext, _settings.JwtCookieName);

        //var accessToken = _accessTokenGenerator.GenerateAccessToken("platform", "access-management");
        //HttpResponseMessage response = await _client.GetAsync(token, endpointUrl, accessToken);
        //string responseContent = await response.Content.ReadAsStringAsync();
        //if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //{
        //    userProfile = JsonSerializer.Deserialize<UserProfile>(responseContent);

        //    // userProfile = await response.Content.ReadAsAsync<UserProfile>();
        //}
        //else
        //{
        //    _logger.LogError($"Getting user profile with userId {userId} failed with statuscode {response.StatusCode}");
        //}

        return userProfile;
    }
}
