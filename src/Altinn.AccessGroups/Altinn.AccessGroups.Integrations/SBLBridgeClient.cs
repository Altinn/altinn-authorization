using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Altinn.AccessGroups.Integrations
{
    /// <summary>
    /// Client for accessing endpoints on AltinnII SBL Bridge 
    /// </summary>
    public class SBLBridgeClient
    {
        /// <summary>
        /// Gets an instance of httpclient from httpclientfactory
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// Initializes the http client for accessing endpoints on AltinnII SBL Bridge 
        /// </summary>
        /// <param name="client">the http client</param>
        /// <param name="settings">the general settings configured for the authorization component</param>
        public SBLBridgeClient(HttpClient client, IOptions<SBLBridgeSettings> settings)
        {
            SBLBridgeSettings sblBridgeSettings = settings.Value;
            Client = client;
            Client.BaseAddress = new Uri(sblBridgeSettings.GetAuthorizationApiEndpoint);
            Client.Timeout = new TimeSpan(0, 0, 30);
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
