using System.Collections.Generic;

namespace Altinn.Platform.Authorization.Models.External
{
    /// <summary>
    /// The JSON Response.
    /// </summary>
    public class XacmlJsonResponseExternal
    {
        /// <summary>
        /// Gets or sets a list over JSON XACML results.
        /// </summary>
        public List<XacmlJsonResultExternal> Response { get; set; }
    }
}
