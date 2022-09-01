using Altinn.AccessGroups.Core.Models;

namespace Altinn.AccessGroups.Core
{
    public interface IAccessGroupsRepository
    {
        /// <summary>
        /// Operation for inserting a new Access Group to the postgreSQL database
        /// </summary>
        /// <param name="accessGroup">The Access Group model to insert</param>
        /// <returns>The resulting Access Group</returns>
        Task<AccessGroup> InsertAccessGroup(AccessGroup accessGroup);

        /// <summary>
        /// Operation for inserting a new external relationship to the postgreSQL database
        /// </summary>
        /// <param name="externalRelationship">The external relationship model to insert</param>
        /// <returns>The resulting external relationship</returns>
        Task<ExternalRelationship> InsertExternalRelationship(ExternalRelationship externalRelationship);
    }
}
