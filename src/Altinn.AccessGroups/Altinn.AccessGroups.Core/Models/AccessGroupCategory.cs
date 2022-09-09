namespace Altinn.AccessGroups.Core.Models
{
    /// <summary>
    /// Model used for the relationship between AccessGroup and Categories in Authorization
    /// </summary>
    public class AccessGroupCategory
    {
        /// <summary>
        /// The Access Group Code
        /// </summary>
        public string AccessGroupCode { get; set; }

        /// <summary>
        /// The Category Code
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// When the Category was created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// When the Category was last modified
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
