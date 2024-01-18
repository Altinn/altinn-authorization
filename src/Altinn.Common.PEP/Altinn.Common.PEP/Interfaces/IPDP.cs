using System.Security.Claims;
using System.Threading.Tasks;

using Altinn.Authorization.ABAC.Xacml.JsonProfile;

namespace Altinn.Common.PEP.Interfaces
{
    /// <summary>
    /// This interface describes the minimum set of methods for any implementation of a Policy Decision Point.
    /// </summary>
    public interface IPDP
    {
        /// <summary>
        /// Sends in a request and get response with result of the request
        /// </summary>
        /// <param name="xacmlJsonRequest">The Xacml Json Request</param>
        /// <param name="forwardedForHeader">x-forwarded-for header value from the client(app)</param>
        /// <returns>The Xacml Json response contains the result of the request</returns>
        Task<XacmlJsonResponse> GetDecisionForRequest(XacmlJsonRequestRoot xacmlJsonRequest, string forwardedForHeader);

        /// <summary>
        /// Change this to a better one???????
        /// </summary>
        /// <param name="xacmlJsonRequest">The Xacml Json Request</param>
        /// <param name="user">The claims principal</param>
        /// <param name="forwardedForHeader">x-forwarded-for header value from the client(app)</param>
        /// <returns>Returns true if request is permitted and false if not</returns>
        Task<bool> GetDecisionForUnvalidateRequest(XacmlJsonRequestRoot xacmlJsonRequest, ClaimsPrincipal user, string forwardedForHeader);
    }
}
