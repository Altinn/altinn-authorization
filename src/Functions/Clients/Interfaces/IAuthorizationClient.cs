using System.Net.Http;
using System.Threading.Tasks;

namespace Altinn.Platform.Authorization.Functions.Clients.Interfaces;

/// <summary>
/// Interface for the authorization client
/// </summary>
public interface IAuthorizationClient
{
    /// <summary>
    /// Performs a POST request to the authorization API with the range of delegation events to replay
    /// </summary>
    /// <param name="startId">The first id in the range to replay</param>
    /// <param name="endId">The last id in the range to replay. If left to 0 all events after the startId will be replayed</param>
    /// <returns>A HTTP response message</returns>
    Task<HttpResponseMessage> PostDelegationEventsReplayAsync(int startId, int endId);
}
