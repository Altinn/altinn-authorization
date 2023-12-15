using System.Collections.Generic;

namespace Altinn.Platform.Authorization.Models.External
{
    /// <summary>
    /// The JSON object Result.
    /// </summary>
    public class XacmlJsonResultExternal
    {
        /// <summary>
        /// Gets or sets the XACML Decision.
        /// </summary>
        public string Decision { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public XacmlJsonStatusExternal Status { get; set;  }

        /// <summary>
        /// Gets or sets any obligations of the result.
        /// </summary>
        public List<XacmlJsonObligationOrAdviceExternal> Obligations { get; set; }

        /// <summary>
        /// Gets or sets xACML Advice.
        /// </summary>
        public List<XacmlJsonObligationOrAdviceExternal> AssociateAdvice { get; set; }

        /// <summary>
        /// Gets or sets category.
        /// </summary>
        public List<XacmlJsonCategoryExternal> Category { get; set;  }

        /// <summary>
        /// Gets or sets policy Identifyer list related to the result.
        /// </summary>
        public XacmlJsonPolicyIdentifierListExternal PolicyIdentifierList { get; set; }
    }
}
