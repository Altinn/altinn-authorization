using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Altinn.Authorization.ABAC.Xacml.JsonProfile;
using Altinn.Common.PEP.Clients;
using Altinn.Common.PEP.Helpers;
using Altinn.Common.PEP.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Altinn.Common.PEP.Implementation
{
    /// <summary>
    /// App implementation of the authorization service where the app uses the Altinn platform api.
    /// </summary>
    public class PDPAppSI : IPDP
    {
        private readonly ILogger _logger;
        private readonly AuthorizationApiClient _authorizationApiClient;
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="PDPAppSI"/> class
        /// </summary>
        /// <param name="logger">the handler for logger service</param>
        /// <param name="authorizationApiClient">A typed Http client accessor</param>
        /// <param name="memoryCache">Memory cache for decision response</param>
        public PDPAppSI(ILogger<PDPAppSI> logger, AuthorizationApiClient authorizationApiClient, IMemoryCache memoryCache)
        {
            _logger = logger;
            _authorizationApiClient = authorizationApiClient;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc/>
        public async Task<XacmlJsonResponse> GetDecisionForRequest(XacmlJsonRequestRoot xacmlJsonRequest)
        {
            XacmlJsonResponse xacmlJsonResponse = null;
            string uniqueCacheKey = System.Text.Json.JsonSerializer.Serialize(xacmlJsonRequest);
            if (_memoryCache.TryGetValue(uniqueCacheKey, out xacmlJsonResponse))
            {
                return xacmlJsonResponse;
            }

            try
            {
                xacmlJsonResponse = await _authorizationApiClient.AuthorizeRequest(xacmlJsonRequest);
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to retrieve Xacml Json response. An error occured {e.Message}");
            }

            if (xacmlJsonResponse != null)
            {
                _memoryCache.Set(uniqueCacheKey, xacmlJsonResponse, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60*5)
                });
            }

            return xacmlJsonResponse;
        }

        /// <inheritdoc/>
        public async Task<bool> GetDecisionForUnvalidateRequest(XacmlJsonRequestRoot xacmlJsonRequest, ClaimsPrincipal user)
        {
            XacmlJsonResponse response = await GetDecisionForRequest(xacmlJsonRequest);

            if (response?.Response == null)
            {
                throw new ArgumentNullException("response");
            }

            _logger.LogInformation($"// Altinn PEP // PDPAppSI // Request sent to platform authorization: {JsonConvert.SerializeObject(xacmlJsonRequest)}");

            return DecisionHelper.ValidatePdpDecision(response.Response, user);
        }
    }
}
