using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Clients.Interfaces;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Services.Interfaces;
using Azure.Messaging;
using Microsoft.Extensions.Azure;

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
    }
}
