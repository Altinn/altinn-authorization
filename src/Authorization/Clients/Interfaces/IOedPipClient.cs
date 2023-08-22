using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Altinn.Platform.Profile.Models;

namespace Altinn.Platform.Authorization.Clients.Interfaces;

/// <summary>
/// Interface for OED PIP client.
/// </summary>
public interface IOedPipClient
{
    ///// <summary>
    ///// Method for getting the userprofile from a given user id
    ///// </summary>
    ///// <param name="userId">the user id</param>
    ///// <returns>The userprofile for the given user id</returns>
    //Task<UserProfile> GetUserProfile(int userId);

    /// <summary>
    /// post async
    /// </summary>
    /// <returns>A HttpResponseMessage<see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<HttpResponseMessage> GetOedRoleAssignments(StringContent requestBody, AuthenticationHeaderValue token);
}
