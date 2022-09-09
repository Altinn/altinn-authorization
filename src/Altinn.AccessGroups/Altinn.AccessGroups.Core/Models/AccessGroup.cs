using System.Text.Json.Serialization;

namespace Altinn.AccessGroups.Core.Models
{
    /// <summary>
    /// Model used for Access Groups in Authorization
    /// </summary>
    public class AccessGroup
    {
        /// <summary>
        /// The Access Group Code
        /// </summary>
        public string AccessGroupCode { get; set; }

        /// <summary>
        /// The Access Group Type
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessGroupType AccessGroupType { get; set; }

        /// <summary>
        /// The list of categories the Access Group is to have as parents
        /// </summary>
        public List<string> Categories { get; set; }

        /// <summary>
        /// Whether the Access Group is hiden
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// When the Access Group was created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// When the Access Group was last modified
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
