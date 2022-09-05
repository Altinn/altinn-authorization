using Altinn.AccessGroups.Core;
using Altinn.AccessGroups.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altinn.AccessGroups.Test.Mocks
{
    internal class AccessGroupRepositoryMock : IAccessGroupsRepository
    {
        public Task<List<AccessGroup>> GetAccessGroups()
        {
            throw new NotImplementedException();
        }

        public Task<List<ExternalRelationship>> GetExternalRelationships()
        {
            throw new NotImplementedException();
        }

        public Task<AccessGroup> InsertAccessGroup(AccessGroup accessGroup)
        {
            throw new NotImplementedException();
        }

        public Task<ExternalRelationship> InsertExternalRelationship(ExternalRelationship externalRelationship)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertGroupMembership(GroupMembership membership)
        {
            throw new NotImplementedException();
        }

        public Task<GroupMembership> ListGroupmemberships()
        {
            throw new NotImplementedException();
        }
    }
}
