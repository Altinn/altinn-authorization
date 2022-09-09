using System.Text.Json.Serialization;

namespace Altinn.AccessGroups.Core.Models
{
    /// <summary>
    /// Model used for representing the relationship between access in an external register to a specific access group in Altinn
    /// </summary>
    public class ExternalRelationship
    {
        /// <summary>
        /// The external register source
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExternalSource ExternalSource { get; set; }

        /// <summary>
        /// The external id
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The Access Group Code
        /// </summary>
        public string AccessGroupCode { get; set; }

        /// <summary>
        /// Unittype filter value if any
        /// </summary>
        public string UnitTypeFilter { get; set; }

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
