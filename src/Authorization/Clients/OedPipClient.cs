using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.Platform.Authorization.Clients.Interfaces;
using Altinn.Platform.Authorization.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Altinn.Platform.Authorization.Clients
{
    /// <summary>
    /// Client configuration for roles api
    /// </summary>
    public class OedPipClient : IOedPipClient
    {
        /// <summary>
        /// Gets an instance of httpclient from httpclientfactory
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// Initializes the http client for roles
        /// </summary>
        /// <param name="client">the http client</param>
        /// <param name="settings">the general settings configured for the authorization component</param>
        public OedPipClient(HttpClient client, IOptions<GeneralSettings> settings)
        {
            GeneralSettings generalSettings = settings.Value;
            Client = client;
            Client.BaseAddress = new Uri(generalSettings.OedPipApiEndpoint);
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// post request that gets OED roleassignments
        /// </summary>
        /// <param name="requestBody">the request body</param>
        /// <param name="token">the bearer token</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> GetOedRoleAssignments(StringContent requestBody, AuthenticationHeaderValue token)
        {
            Client.DefaultRequestHeaders.Authorization = token;
            string endpoint = Client.BaseAddress + "v1/pip";
            return await Client.PostAsync(endpoint, requestBody);
        }
    }
}