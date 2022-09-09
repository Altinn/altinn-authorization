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
        /// Operation for getting all access groups from the postgreSQL database
        /// </summary>
        /// <returns>The list of access groups</returns>
        Task<List<AccessGroup>> GetAccessGroups();

        /// <summary>
        /// Operation for inserting a new external relationship to the postgreSQL database
        /// </summary>
        /// <param name="externalRelationship">The external relationship model to insert</param>
        /// <returns>The resulting external relationship</returns>
        Task<ExternalRelationship> InsertExternalRelationship(ExternalRelationship externalRelationship);

        /// <summary>
        /// Operation for getting all external relationship from the postgreSQL database
        /// </summary>
        /// <returns>The list of external relationships</returns>
        Task<List<ExternalRelationship>> GetExternalRelationships();

        /// <summary>
        /// Operation for inserting a new category into the postgreSQL database
        /// </summary>
        /// <param name="category">The vategory model to insert</param>
        /// <returns>The resulting category</returns>
        Task<Category> InsertCategory(Category category);

        /// <summary>
        /// Operation for getting all categories from the postgreSQL database
        /// </summary>
        /// <returns>The list of categories</returns>
        Task<List<Category>> GetCategories();

        /// <summary>
        /// Operation for inserting group membership in the postgreSQL database
        /// </summary>
        /// <returns>The list of access groups</returns>
        Task<bool> InsertGroupMembership(GroupMembership membership);

        /// <summary>
        /// Operation for selecting group memberships in the postgreSQL database
        /// </summary>
        /// <returns>The list of access groups</returns>
        Task<List<GroupMembership>> ListGroupmemberships();
    }
}
