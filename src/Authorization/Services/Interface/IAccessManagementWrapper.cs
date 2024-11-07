using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Models.AccessManagement;

namespace Altinn.Platform.Authorization.Services.Interface
{
    /// <summary>
    /// The service used to map internal delegation change to delegation change events and push them to the event queue.
    /// </summary>
    public interface IAccessManagementWrapper
    {
        /// <summary>
        /// Endpoint to find all delegation changes for a given user, reportee and app/resource context
        /// </summary>
        /// <returns>Input parameter to the request</returns>
        public Task<IEnumerable<DelegationChangeExternal>> GetAllDelegationChanges(DelegationChangeInput input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Endpoint to find all delegation changes for a given user, reportee and app/resource context
        /// </summary>
        /// <returns>optional funvation pattern for modifying the request sent to Access Management API</returns>
        public Task<IEnumerable<DelegationChangeExternal>> GetAllDelegationChanges(CancellationToken cancellationToken = default, params Action<DelegationChangeInput>[] actions);

        /// <summary>
        /// Endpoint to get the list of all authorized parties for the authenticated user
        /// </summary>
        /// <returns>Enumerable of all the parties the user have access to</returns>
        public Task<IEnumerable<AuthorizedPartyDto>> GetAuthorizedParties(CancellationToken cancellationToken = default);
    }
}
