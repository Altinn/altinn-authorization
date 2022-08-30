using Altinn.AccessGroups.Core;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Altinn.AccessGroups.Integrations
{
    /// <summary>
    /// Client for getting Altinn roles from AltinnII SBL Bridge
    /// </summary>
    public class AltinnRolesClient : IAltinnRolesClient
    {
        /// <summary>
        /// Gets an instance of httpclient from httpclientfactory
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// Initializes the http client for getting roles from AltinnII SBL Bridge 
        /// </summary>
        /// <param name="client">the http client</param>
        /// <param name="settings">the general settings configured for the authorization component</param>
        public AltinnRolesClient(HttpClient client, IOptions<SBLBridgeSettings> settings)
        {
            SBLBridgeSettings sblBridgeSettings = settings.Value;
            Client = client;
            Client.BaseAddress = new Uri(sblBridgeSettings.GetBridgeApiEndpoint);
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
