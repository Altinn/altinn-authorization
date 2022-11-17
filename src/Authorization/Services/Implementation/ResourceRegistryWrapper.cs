using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Clients;
using Altinn.Platform.Authorization.Helpers;
using Altinn.Platform.Authorization.Services.Interface;

namespace Altinn.Platform.Authorization.Services.Implementation
{
    /// <summary>
    /// Wrapper for resource registry
    /// </summary>
    public class ResourceRegistryWrapper : IResourceRegistry
    {
        private readonly ResourceRegistryClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRegistryWrapper"/> class.
        /// </summary>
        /// <param name="resourceRegistryClient">The httpclient</param>
        public ResourceRegistryWrapper(ResourceRegistryClient  resourceRegistryClient)
        {
            _client = resourceRegistryClient;
        }

        /// <inheritdoc/>
        public async Task<XacmlPolicy> GetResourcePolicyAsync(string resourceId)
        {
            XacmlPolicy policy = null;
            string apiurl = $"resorces/{resourceId}/policy";
            HttpResponseMessage response = await _client.Client.GetAsync(apiurl);
        
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Stream policyBlob = await response.Content.ReadAsStreamAsync();
                using (policyBlob)
                {
                    policy = (policyBlob.Length > 0) ? PolicyHelper.ParsePolicy(policyBlob) : null;
                }
            }

            return policy;
        }
    }
}
