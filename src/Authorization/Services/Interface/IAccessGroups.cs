using System.Collections.Generic;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Models;

namespace Altinn.Platform.Authorization.Services.Interface
{
    /// <summary>
    /// Interface for Access groups
    /// </summary>
    public interface IAccessGroups
    {
        /// <summary>
        /// List all memberships
        /// </summary>
        Task<List<AccessGroupMembership>> GetMemberships(int? memberUserId, int? memberPartyId, int offeredByPartyId);
    }
}
