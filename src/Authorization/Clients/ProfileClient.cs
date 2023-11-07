using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Altinn.Platform.Authorization.Configuration;
using Microsoft.Extensions.Options;

namespace Altinn.Platform.Authorization.Clients
{
    /// <summary>
    /// Client configuration for profile api
    /// </summary>
    public class ProfileClient
    {        
        /// <summary>
        /// Gets an instance of httpclient from httpclientfactory
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// Initializes the http client for actor
        /// </summary>
        /// <param name="client">the http client</param>
        /// <param name="generalSettings">the general settings configured for the authorization component</param>
        public ProfileClient(HttpClient client, IOptions<GeneralSettings> generalSettings)
        {
            GeneralSettings settings = generalSettings.Value;
            Client = client;
            Client.BaseAddress = new Uri(settings.ProfileApiEndpoint);
            Client.Timeout = new TimeSpan(0, 0, 30);
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
