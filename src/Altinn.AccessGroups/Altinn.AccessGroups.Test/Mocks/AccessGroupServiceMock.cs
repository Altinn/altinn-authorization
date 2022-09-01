using Altinn.AccessGroups.Interfaces;
using Altinn.AccessGroups.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altinn.AccessGroups.Core.Models;

namespace Altinn.AccessGroups.Test.Mocks
{
    internal class AccessGroupServiceMock : IAccessGroup
    {
        public Task<bool> AddMembership(GroupMembership input)
        {
            throw new NotImplementedException();
        }

        public Task<AccessGroup> CreateGroup(AccessGroup accessGroup)
        {
            throw new NotImplementedException();
        }

        public Task<List<AccessGroup>> ExportAccessGroups()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ImportAccessGroups(List<AccessGroup> accessGroups)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExternalRelationship>> ImportExternalRelationships(List<ExternalRelationship> externalRelationships)
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

        public Task<bool> UpdateGroup(AccessGroup accessGroup)
        {
            throw new NotImplementedException();
        }
    }
}
