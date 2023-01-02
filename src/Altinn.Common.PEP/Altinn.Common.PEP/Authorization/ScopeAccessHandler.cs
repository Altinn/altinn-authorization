using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Altinn.Common.PEP.Authorization
{
    /// <summary>
    /// Represents an authorization handler that can perform authorization based on scope
    /// </summary>
    public class ScopeAccessHandler : AuthorizationHandler<ScopeAccessRequirement>
    {
        private readonly ILogger _logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeAccessHandler"/> class.
        /// </summary>
        /// <param name="logger">A logger from the logger factory</param>
        public ScopeAccessHandler(ILogger<ScopeAccessHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Performs necessary logic to evaluate the scope requirement.
        /// </summary>
        /// <param name="context">The current <see cref="AuthorizationHandlerContext"/></param>
        /// <param name="requirement">The scope requirement to evaluate.</param>
        /// <returns>Returns a Task for async await</returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeAccessRequirement requirement)
        {
            // get scope parameter from  user claims
            string contextScope = context?.User?.Identities 
                ?.FirstOrDefault(i => i.AuthenticationType != null && i.AuthenticationType.Equals("AuthenticationTypes.Federation"))?.Claims
                .Where(c => c.Type.Equals("urn:altinn:scope"))?
                .Select(c => c.Value).FirstOrDefault();

            contextScope ??= context?.User?.Claims.Where(c => c.Type.Equals("scope")).Select(c => c.Value).FirstOrDefault();

            bool validScope = false;

            // compare scope claim value to
            if (!string.IsNullOrWhiteSpace(contextScope))
            {
                string[] requiredScopes = requirement.Scope;
                List<string> clientScopes = contextScope?.Split(' ').ToList();

                foreach (string requiredScope in requiredScopes)
                {
                    if (clientScopes.Contains(requiredScope))
                    {
                        validScope = true;
                        break;
                    }
                }
            }

            if (validScope)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            await Task.CompletedTask;
        }
    }
}
