using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.ApiClients.Maskinporten.Config;
using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.ApiClients.Maskinporten.Services;
using Altinn.Platform.Authorization.Clients;
using Altinn.Platform.Authorization.Clients.Interfaces;
using Altinn.Platform.Authorization.Configuration;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Services.Interface;
using Azure.Core;
using Microsoft.Extensions.Options;

namespace Altinn.Platform.Authorization.Services.Implementation
{
    /// <summary>
    /// Wrapper for Access groups component
    /// </summary>
    public class OedRoleAssignmentWrapper : IOedRoleAssignmentWrapper
    {
        private readonly HttpClient _httpClient;
        private readonly IMaskinportenService _maskinPortenService;
        private readonly IOptions<MaskinportenSettings> _maskinportenSettings;
        private readonly IOptions<GeneralSettings> _generalSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesWrapper"/> class
        /// </summary>
        /// <param name="client">the client handler for OED api</param>
        /// <param name="generalSettings">the general settings</param>
        /// <param name="maskinportenService">the maskinporten service</param>
        /// <param name="maskinportenSettings">the maskinporten settings</param>
        public OedRoleAssignmentWrapper(HttpClient client, IOptions<GeneralSettings> generalSettings, IMaskinportenService maskinportenService, IOptions<MaskinportenSettings> maskinportenSettings)
        {
            _httpClient = client;
            _maskinPortenService = maskinportenService;
            _generalSettings = generalSettings;
            _maskinportenSettings = maskinportenSettings;
        }

        /// <inheritdoc/>
        public async Task<List<OedRoleAssignment>> GetOedRoleAssignments(string from, string to)
        {
            try
            {
                MaskinportenSettings mpsettings = _maskinportenSettings.Value;
                GeneralSettings gensettings = _generalSettings.Value;
                TokenResponse tokenResponse = await _maskinPortenService.GetToken(mpsettings.EncodedJwk, mpsettings.Environment, mpsettings.ClientId, mpsettings.Scope, string.Empty);

                OedRoleAssignments oedRoleAssignments = new() { RoleAssignments = new List<OedRoleAssignment>() };
                OedRoleAssignmentRequest oedRoleAssignmentRequest = new OedRoleAssignmentRequest
                {
                    From = from,
                    To = to
                };

                string endpoint = _generalSettings.Value.OedPipApiEndpoint + "v1/pip";
                
                // AuthenticationHeaderValue token = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                StringContent requestBody = new StringContent(JsonSerializer.Serialize(oedRoleAssignmentRequest), Encoding.UTF8, "application/json");

                // HttpResponseMessage response = await _httpClient.GetOedRoleAssignments(requestBody, token);

                HttpResponseMessage response = await _httpClient.PostAsync(endpoint, requestBody);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    oedRoleAssignments = JsonSerializer.Deserialize<OedRoleAssignments>(responseContent, options);
                }

                return oedRoleAssignments.RoleAssignments;
            }
            catch (System.Exception e)
            {
            }

            return null;
        }
    }
}