using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Services.Interface;

using Authorization.Platform.Authorization.Models;

namespace Altinn.Platform.Authorization.IntegrationTests.MockServices
{
    public class AccessGroupsMock : IAccessGroups
    {
        public Task<List<AccessGroupMembership>> GetMemberships(int? memberUserId, int? memberPartyId, int offeredByPartyId)
        {
            List<AccessGroupMembership> memberships = new List<AccessGroupMembership>();
            string rolesPath = GetMembershipPath(memberUserId.Value, offeredByPartyId);
            if (File.Exists(rolesPath))
            {
                string content = File.ReadAllText(rolesPath);
                memberships = (List<AccessGroupMembership>)JsonSerializer.Deserialize(content, typeof(List<AccessGroupMembership>), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return Task.FromResult(memberships);
        }

        private static string GetMembershipPath(int coveredByUserId, int offeredByPartyId)
        {
            string unitTestFolder = Path.GetDirectoryName(new Uri(typeof(RolesMock).Assembly.Location).LocalPath);
            return Path.Combine(unitTestFolder, "..", "..", "..", "Data", "Roles", $"user_{coveredByUserId}", $"party_{offeredByPartyId}", "memberships.json");
        }
    }
}
