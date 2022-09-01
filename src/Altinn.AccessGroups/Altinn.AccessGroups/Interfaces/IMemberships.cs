﻿using Altinn.AccessGroups.Core.Models;

namespace Altinn.AccessGroups.Interfaces
{
    public interface IMemberships
    {
        Task<List<AccessGroup>> ListGroupMemberships(AccessGroupSearch search);

        Task<bool> AddMembership(GroupMembership input);

        Task<bool> RevokeMembership(GroupMembership input);
    }
}
