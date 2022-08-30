using Authorization.Platform.Authorization.Models;
using Altinn.AccessGroups.Core;
using System.Text.Json;

namespace Altinn.AccessGroups.Integrations
{
    /// <summary>
    /// Client for getting Altinn roles from AltinnII SBL Bridge
    /// </summary>
    public class AltinnRolesClient : IAltinnRolesClient
    {
        private readonly SBLBridgeClient _bridgeClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesWrapper"/> class
        /// </summary>
        /// <param name="bridgeClient">the client handler for roles api</param>
        public AltinnRolesClient(SBLBridgeClient bridgeClient)
        {
            _bridgeClient = bridgeClient;
        }

        /// <inheritdoc />
        public async Task<List<Role>> GetDecisionPointRolesForUser(int coveredByUserId, int offeredByPartyId)
        {
            List<Role> decisionPointRoles = new List<Role>();
            string apiurl = $"roles?coveredByUserId={coveredByUserId}&offeredByPartyId={offeredByPartyId}";

            HttpResponseMessage response = await _bridgeClient.Client.GetAsync(apiurl);
            string roleList = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                decisionPointRoles = JsonSerializer.Deserialize<List<Role>>(roleList);
            }

            return decisionPointRoles;
        }
    }
}
