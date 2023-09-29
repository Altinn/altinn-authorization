using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Clients.Interfaces;
using Altinn.Platform.Authorization.Configuration;
using Altinn.Platform.Authorization.Helpers;
using Altinn.Platform.Authorization.Models.EventLog;
using Altinn.Platform.Authorization.Services.Interfaces;
using Azure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Azure;
using Microsoft.FeatureManagement;

namespace Altinn.Platform.Authorization.Services.Implementation
{
    /// <summary>
    /// Implementation for authentication event log
    /// </summary>
    public class EventLogService : IEventLog
    {
        private readonly IEventsQueueClient _queueClient;

        /// <summary>
        /// Instantiation for event log servcie
        /// </summary>
        /// <param name="queueClient">queue client to store event in event log</param>
        public EventLogService(IEventsQueueClient queueClient)
        {
            _queueClient = queueClient;
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
                AuthorizationEvent authorizationEvent = EventLogHelper.MapAuthorizationEventFromContextRequest(contextRequest, context, contextResponse);

                if (authorizationEvent != null)
                {
                    _queueClient.EnqueueAuthorizationEvent(JsonSerializer.Serialize(authorizationEvent));
                }
            }
        }
    }
}
