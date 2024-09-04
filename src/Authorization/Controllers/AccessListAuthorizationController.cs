using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Altinn.Authorization.Errors;
using Altinn.Authorization.Models.ResourceRegistry;
using Altinn.Authorization.ProblemDetails;
using Altinn.Platform.Authorization.Constants;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Services.Interface;
using AltinnCore.Authentication.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Altinn.Platform.Authorization.Controllers;

/// <summary>
/// Contains all actions related to the roles model
/// </summary>
[Route("authorization/api/v1/accesslist")]
[ApiController]
public class AccessListAuthorizationController : ControllerBase
{
    private readonly IAccessListAuthorization _accessListAuthorization;
    private readonly IResourceRegistry _resourceRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessListAuthorizationController"/> class
    /// </summary>
    public AccessListAuthorizationController(IAccessListAuthorization accessListAuthorization, IResourceRegistry resourceRegistry)
    {
        _accessListAuthorization = accessListAuthorization;
        _resourceRegistry = resourceRegistry;
    }

    /// <summary>
    /// Authorization of a given subject for resource access through access lists.
    /// </summary>
    /// <param name="accessListAuthorizationRequest">Access list authorization request model</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>AccessListAuthorizationResponse</returns>
    [HttpPost]
    [Route("authorize")]
    [Authorize(Policy = AuthzConstants.AUTHORIZESCOPEACCESS)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AccessListAuthorizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(AltinnValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDescriptor), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Authorize(AccessListAuthorizationRequest accessListAuthorizationRequest, CancellationToken cancellationToken = default)
    {
        var scopes = HttpContext.User.Claims.First(c => c.Type.Equals("scope"));
        bool isAdmin = scopes.Value.Contains(AuthzConstants.AUTHORIZE_ADMIN_SCOPE);

        if (!isAdmin)
        {
            var orgCode = HttpContext.User?.Claims.FirstOrDefault(c => c.Type.Equals(AltinnCoreClaimTypes.Org));
            var orgNumber = HttpContext.User?.Claims.FirstOrDefault(c => c.Type.Equals(AltinnCoreClaimTypes.OrgNumber));

            if (string.IsNullOrWhiteSpace(orgCode?.Value) || string.IsNullOrWhiteSpace(orgNumber?.Value))
            {
                return Forbid();
            }

            ServiceResource resource = await _resourceRegistry.GetResourceAsync(accessListAuthorizationRequest.Resource.Value.ValueSpan.ToString(), cancellationToken);
            if (resource == null)
            {
                return new AltinnValidationProblemDetails([
                    ValidationErrors.ResourceRegistry_ResourceIdentifier_NotFound.ToValidationError($"/resource/value"),
                ]).ToActionResult();
            }

            if (resource.HasCompetentAuthority.Organization != orgNumber.Value)
            {
                return new AltinnValidationProblemDetails([
                    ValidationErrors.ResourceRegistry_CompetentAuthority_NotMatchingAuthenticatedOrganization.ToValidationError($"/HEADER/authorization/bearer/{AltinnCoreClaimTypes.OrgNumber}"),
                ]).ToActionResult();
            }

            if (resource.HasCompetentAuthority.Orgcode != orgCode.Value)
            {
                return new AltinnValidationProblemDetails([
                    ValidationErrors.ResourceRegistry_CompetentAuthority_NotMatchingAuthenticatedOrgCode.ToValidationError($"/HEADER/authorization/bearer/{AltinnCoreClaimTypes.Org}"),
                ]).ToActionResult();
            }
        }

        return await AuthorizeInternal(accessListAuthorizationRequest, cancellationToken);
    }

    /// <summary>
    /// Internal API for local cluster requests only.
    /// Authorization of a given subject for resource access through access lists for any service owner organization's access lists and resources. 
    /// </summary>
    /// <param name="accessListAuthorizationRequest">Access list authorization request model</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>AccessListAuthorizationResponse</returns>
    [HttpPost]
    [Route("authorize/internal")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AccessListAuthorizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AuthorizeInternal(AccessListAuthorizationRequest accessListAuthorizationRequest, CancellationToken cancellationToken = default)
    {
        Result<AccessListAuthorizationResponse> result = await _accessListAuthorization.Authorize(accessListAuthorizationRequest, cancellationToken);

        if (result.IsProblem)
        {
            return result.Problem.ToActionResult();
        }

        return Ok(result.Value);
    }
}
