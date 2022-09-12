using System.Text.Json.Serialization;

namespace Altinn.AccessGroups.Core.Models
{
    /// <summary>
    /// Model used for Categories of AccessGroups in Authorization
    /// </summary>
    public class Category
    {
        /// <summary>
        /// The Category Code
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// The type of category tree the category belongs to
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CategoryType CategoryType { get; set; }

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
