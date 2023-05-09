using System.Threading.Tasks;

namespace Altinn.Platform.Authorization.Functions.Services.Interfaces;

/// <summary>
/// The service uses to call platform authorization to trigger replay of events
/// </summary>
public interface IEventReplayService
{
    /// <summary>
    /// Calls the platform authorization API for replaying events from the database to the event queue
    /// </summary>
    Task ReplayEvents(int startId, int endId);
}
