using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Clients;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Services.Interface;
using Newtonsoft.Json;

namespace Altinn.Platform.Authorization.Services.Implementation
{
    /// <summary>
    /// Wrapper for Access groups component
    /// </summary>
    public class AccessGroupsWrapper : IAccessGroups
    {
        private readonly AccessGroupsClient _accessGroupsClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesWrapper"/> class
        /// </summary>
        /// <param name="accessGroupsClient">the client handler for accessGro api</param>
        public AccessGroupsWrapper(AccessGroupsClient accessGroupsClient)
        {
            _accessGroupsClient = accessGroupsClient;
        }

        /// <inheritdoc/>
        public async Task<List<AccessGroupMembership>> GetMemberships(int? memberUserId, int? memberPartyId, int offeredByPartyId)
        {
            List<AccessGroupMembership> accessgroupsMemberships = new List<AccessGroupMembership>();
            string apiurl = $"roles?coveredByUserId={memberUserId}&offeredByPartyId={offeredByPartyId}";

            HttpResponseMessage response = await _accessGroupsClient.Client.GetAsync(apiurl);
            string accessGroupsList = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                accessgroupsMemberships = JsonConvert.DeserializeObject<List<AccessGroupMembership>>(accessGroupsList);
            }

            return accessgroupsMemberships;
        }
    }
}
