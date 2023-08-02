using System.Collections.Generic;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Models;

namespace Altinn.Platform.Authorization.Services.Interface
{
    /// <summary>
    /// Service mapping OED role assignments
    /// </summary>
    public interface IOedRoleAssignmentWrapper
    {
        /// <summary>
        /// Gets OED role assignments between the deceased party and the inheriting party
        /// </summary>
        /// <param name="from">the deceased party</param>
        /// <param name="to"> the inheriting party</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public Task<List<OedRoleAssignment>> GetOedRoleAssignments(string from, string to);
    }
}