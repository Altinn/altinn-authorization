using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Clients.Interfaces;
using Altinn.Platform.Authorization.Configuration;
using Altinn.Platform.Authorization.Helpers;
using Altinn.Platform.Authorization.Models.EventLog;
using Altinn.Platform.Authorization.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;

namespace Altinn.Platform.Authorization.Services.Implementation
{
    /// <summary>
    /// Implementation for authentication event log
    /// </summary>
    public class EventLogService : IEventLog
    {
        private readonly IEventsQueueClient _queueClient;
        private readonly ISystemClock _systemClock;

        /// <summary>
        /// Instantiation for event log servcie
        /// </summary>
        /// <param name="queueClient">queue client to store event in event log</param>
        /// <param name="systemClock">handler for datetime service</param>
        public EventLogService(IEventsQueueClient queueClient, ISystemClock systemClock)
        {
            _queueClient = queueClient;
            _systemClock = systemClock;
        }

        /// <summary>
        /// Queues an authorization event to the logqueue
        /// </summary>
        /// <param name="authorizationEvent">authorization event</param>
        public void CreateAuthorizationEvent(AuthorizationEvent authorizationEvent)
        {
            if (authorizationEvent != null)
            {
                _queueClient.EnqueueAuthorizationEvent(JsonSerializer.Serialize(authorizationEvent));
            }
        }

        /// <inheritdoc />
        public async Task CreateAuthorizationEvent(IFeatureManager featureManager, XacmlContextRequest contextRequest, HttpContext context, XacmlContextResponse contextResponse)
        {
            if (await featureManager.IsEnabledAsync(FeatureFlags.AuditLog))
            {
                AuthorizationEvent authorizationEvent = EventLogHelper.MapAuthorizationEventFromContextRequest(contextRequest, context, contextResponse, _systemClock.UtcNow.DateTime);

                if (authorizationEvent != null)
                {
                    _queueClient.EnqueueAuthorizationEvent(JsonSerializer.Serialize(authorizationEvent));
                }
            }
        }
    }
}
