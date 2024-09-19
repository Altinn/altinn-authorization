using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Common.AccessTokenClient.Services;
using Altinn.Platform.Authorization.Configuration;
using Altinn.Platform.Authorization.Exceptions;
using Altinn.Platform.Authorization.Extensions;
using Altinn.Platform.Authorization.Services.Interfaces;
using Altinn.Platform.Register.Models;
using AltinnCore.Authentication.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Altinn.Platform.Authorization.Services
{
    /// <summary>
    /// Handles register service
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RegisterService : IRegisterService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GeneralSettings _generalSettings;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly ILogger<IRegisterService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterService"/> class.
        /// </summary>
        public RegisterService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            IAccessTokenGenerator accessTokenGenerator,
            IOptions<GeneralSettings> generalSettings,
            IOptions<PlatformSettings> platformSettings,
            ILogger<IRegisterService> logger,
            IMemoryCache memoryCache)
        {
            httpClient.BaseAddress = new Uri(platformSettings.Value.ApiRegisterEndpoint);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _generalSettings = generalSettings.Value;
            _accessTokenGenerator = accessTokenGenerator;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc/>
        public async Task<Party> GetParty(int partyId)
        {
            string cacheKey = $"p:{partyId}";
            if (!_memoryCache.TryGetValue(cacheKey, out Party party))
            {
                string endpointUrl = $"parties/{partyId}";
                string token = JwtTokenUtil.GetTokenFromContext(_httpContextAccessor.HttpContext, _generalSettings.RuntimeCookieName);
                string accessToken = _accessTokenGenerator.GenerateAccessToken("platform", "authorization");

                HttpResponseMessage response = await _client.GetAsync(token, endpointUrl, accessToken);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    party = JsonSerializer.Deserialize<Party>(responseContent, _serializerOptions);
                    PutInCache(cacheKey, 10, party);
                }
                else
                {
                    _logger.LogError("// Getting party with partyID {partyId} failed with statuscode {response.StatusCode}", partyId, response.StatusCode);
                }
            }

            return party;
        }

        /// <inheritdoc/>
        public async Task<Party> PartyLookup(string orgNo, string person)
        {
            string cacheKey;
            PartyLookup partyLookup;

            if (!string.IsNullOrWhiteSpace(orgNo))
            {
                cacheKey = $"org:{orgNo}";
                partyLookup = new PartyLookup { OrgNo = orgNo };
            }
            else if (!string.IsNullOrWhiteSpace(person))
            {
                cacheKey = $"fnr:{person}";
                partyLookup = new PartyLookup { Ssn = person };
            }
            else
            {
                return null;
            }

            if (!_memoryCache.TryGetValue(cacheKey, out Party party))
            {
                string endpointUrl = "parties/lookup";

                string bearerToken = JwtTokenUtil.GetTokenFromContext(_httpContextAccessor.HttpContext, _generalSettings.RuntimeCookieName);
                string accessToken = _accessTokenGenerator.GenerateAccessToken("platform", "authorization");

                StringContent content = new StringContent(JsonSerializer.Serialize(partyLookup));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                HttpResponseMessage response = await _client.PostAsync(endpointUrl, content, bearerToken, accessToken);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    party = JsonSerializer.Deserialize<Party>(responseContent, _serializerOptions);
                    PutInCache(cacheKey, 10, party);
                }
                else
                {
                    string reason = await response.Content.ReadAsStringAsync();
                    _logger.LogError("// RegisterService // PartyLookup // Failed to lookup party in platform register. Response {response}. \n Reason {reason}.", response, reason);

                    throw await PlatformHttpException.CreateAsync(response);
                }
            }

            return party;
        }

        private void PutInCache(string cachekey, int cacheTimeout, object cacheObject)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
               .SetPriority(CacheItemPriority.High)
               .SetAbsoluteExpiration(new TimeSpan(0, cacheTimeout, 0));

            _memoryCache.Set(cachekey, cacheObject, cacheEntryOptions);
        }
    }
}
