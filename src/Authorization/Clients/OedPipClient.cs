using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;
using Altinn.Platform.Authorization.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Altinn.Platform.Authorization.Clients
{
    /// <summary>
    /// Client configuration for roles api
    /// </summary>
    public class OedPipClient
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
            string accessToken = "abc";
            Client = client;
            Client.BaseAddress = new Uri(generalSettings.OedPipApiEndpoint);
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}