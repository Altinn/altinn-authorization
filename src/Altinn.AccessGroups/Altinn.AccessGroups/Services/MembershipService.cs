using Altinn.AccessGroups.Core;
using Altinn.AccessGroups.Core.Models;
using Altinn.AccessGroups.Interfaces;
using Authorization.Platform.Authorization.Models;

namespace Altinn.AccessGroups.Services
{
    public class MembershipService : IMemberships
    {
        private IAltinnRolesClient _rolesClient;
        public MembershipService(IAltinnRolesClient rolesClient)
        {
            _rolesClient = rolesClient;
        }

        public async Task<bool> AddMembership(GroupMembership input)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AccessGroup>> ListGroupMemberships(AccessGroupSearch search)
        {
            if(!search.CoveredByUserId.HasValue)
            {
                throw new NotImplementedException();
            }

            List<Role> erRoles = await _rolesClient.GetDecisionPointRolesForUser((int)search.CoveredByUserId, search.OfferedByPartyId);

            List<AccessGroup> accessGroups = new List<AccessGroup>();

            return accessGroups;
        }
        
        public async Task<bool> RevokeMembership(GroupMembership input)
        {
            throw new NotImplementedException();
        }
    }
}
