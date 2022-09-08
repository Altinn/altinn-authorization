namespace Altinn.AccessGroups.Core.Models
{
    /// <summary>
    /// The type of a TextResource
    /// </summary>
    public enum TextResourceType
    {
        /// <summary>
        /// Undefined default value
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Text resources tied to an access group definition
        /// </summary>
        AccessGroup = 1,

        /// <summary>
        /// Text resources tied to a category definition
        /// </summary>
        Category = 2
    }
}
