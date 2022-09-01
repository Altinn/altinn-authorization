using Altinn.AccessGroups.Core.Models;

namespace Altinn.AccessGroups.Interfaces
{
    public interface IAccessGroup
    {
        Task<AccessGroup> CreateGroup(AccessGroup accessGroup);

        Task<bool> UpdateGroup(AccessGroup accessGroup);

        Task<List<AccessGroup>> GetAccessGroups();

        Task<List<AccessGroup>> ImportAccessGroups(List<AccessGroup> accessGroups);

        Task<List<ExternalRelationship>> ImportExternalRelationships(List<ExternalRelationship> externalRelationships);

        Task<List<ExternalRelationship>> GetExternalRelationships();
    }
}
