using System.Threading.Tasks;
using Altinn.Platform.Authorization.Models;

namespace Altinn.Platform.Authorization.Services.Interfaces
{
    /// <summary>
    /// Defines event log interface to queue an authentication event to a storage queue
    /// </summary>
    public interface IEventLog
    {
        /// <summary>
        /// Creates an authorization event in storage queue
        /// </summary>
        /// <param name="authorizationEvent">authorization event</param>
        public void CreateAuthorizationEvent(AuthorizationEvent authorizationEvent);
    }
}
