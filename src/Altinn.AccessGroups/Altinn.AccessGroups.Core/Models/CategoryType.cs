namespace Altinn.AccessGroups.Core.Models
{
    /// <summary>
    /// The type of category-tree type a category belongs to
    /// </summary>
    public enum CategoryType
    {
        /// <summary>
        /// Undefined default value
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// For categories belonging to the category-tree for organization
        /// </summary>
        Organization = 1,

        // <summary>
        /// For categories belonging to the category-tree for private individuals
        /// </summary>
        Person = 2
    }
}
