using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Functions.Clients.Interfaces;
using Altinn.Platform.Authorization.Functions.Exceptions;
using Altinn.Platform.Authorization.Functions.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Altinn.Platform.Authorization.Functions.Services;

/// <inheritdoc />
public class EventReplayService : IEventReplayService
{
    private readonly ILogger _logger;
    private readonly IAuthorizationClient _authorizationClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventReplayService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="authorizationClient">The authorization client.</param>
    public EventReplayService(ILogger<EventPusherService> logger, IAuthorizationClient authorizationClient)
    {
        _logger = logger;
        _authorizationClient = authorizationClient;
    }

    /// <summary>
    /// Calls the platform authorization API for replaying events from the database to the event queue
    /// </summary>
    /// <param name="startId">The first id in the range to replay</param>
    /// <param name="endId">The last id in the range to replay. If left to 0 all events after the startId will be replayed</param>
    public async Task ReplayEvents(int startId, int endId)
    {
        try
        {
            if (startId <= 0)
            {
                throw new ArgumentException($"Must specify a valid starting delegation change id for replay. Invalid value: {startId}");
            }

            if (endId != 0 && endId < startId)
            {
                throw new ArgumentException($"The endId cannot be smaller than the startId. startId: {startId}, endId: {endId}");
            }

            HttpResponseMessage response = await _authorizationClient.PostDelegationEventsReplayAsync(startId, endId);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Authorization returned non-success. resultCode={resultCode} reasonPhrase={reasonPhrase} resultBody={resultBody} startId={startId} endId={endId}",
                    response.StatusCode,
                    response.ReasonPhrase,
                    await response.Content.ReadAsStringAsync(),
                    startId,
                    endId);

                throw new AuthorizationRequestFailedException();
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "Successfully posted replay of delegation events for the range startId={startId} endId={endId}",
                    startId,
                    endId);
            }
        }
        catch (AuthorizationRequestFailedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Exception thrown while attempting to post replay of delegation events to Authorization for the range startId={startId} endId={endId}. exception={exception} message={message}",
                startId,
                endId,
                ex.GetType().Name,
                ex.Message);

            throw;
        }
    }
}
