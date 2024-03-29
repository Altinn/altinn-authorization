using Microsoft.AspNetCore.Authorization;

namespace Altinn.Common.PEP.Authorization
{
    /// <summary>
    /// Requirement for authorization policies used for accessing apps.
    /// <see href="https://docs.asp.net/en/latest/security/authorization/policies.html"/> for details about authorization
    /// in asp.net core.
    /// </summary>
    public class ResourceAccessRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAccessRequirement"/> class
        /// </summary>
        /// <param name="actionType">The Action type for this requirement</param>
        /// <param name="resourceId">The resource id for the resource authorization is verified for</param>
        public ResourceAccessRequirement(string actionType, string resourceId)
        {
            this.ActionType = actionType;
            this.ResourceId = resourceId;
        }

        /// <summary>
        /// Gets or sets The Action type defined for the policy using this requirement
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// Gets or sets the resourcId for the resource that authorization should verified for
        /// </summary>
        public string ResourceId { get; set; }
    }
}
