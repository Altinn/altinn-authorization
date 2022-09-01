using Altinn.AccessGroups.Core.Models;
using Altinn.AccessGroups.Interfaces;

namespace Altinn.AccessGroups.Services
{
    public class MembershipService : IMemberships
    {
        public Task<bool> AddMembership(GroupMembership input)
        {
            throw new NotImplementedException();
        }

        public Task<List<AccessGroup>> ListGroupMemberships(AccessGroupSearch search)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RevokeMembership(GroupMembership input)
        {
            throw new NotImplementedException();
        }
    }
}
