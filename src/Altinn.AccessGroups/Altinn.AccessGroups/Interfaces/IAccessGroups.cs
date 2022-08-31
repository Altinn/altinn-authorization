using Altinn.AccessGroups.Core.Models;

namespace Altinn.AccessGroups.Interfaces
{
    public interface IAccessGroup
    {
        Task<List<AccessGroup>> ListGroupMemberships(AccessGroupSearch search);

        Task<bool> AddMembership(GroupMembership input);

        Task<bool> RevokeMembership(GroupMembership input);

        Task<bool> CreateGroup(AccessGroup accessGroup);

        Task<bool> UpdateGroup(AccessGroup accessGroup);

        Task<List<AccessGroup>> ExportAccessGroups();

        Task<bool> ImportAccessGroups(List<AccessGroup> accessGroups);
    }
}
