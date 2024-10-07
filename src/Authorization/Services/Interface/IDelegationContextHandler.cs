using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Models;

namespace Altinn.Platform.Authorization.Services.Interface
{
    /// <summary>
    /// Defines Interface for the Delegation Context Handler.
    /// </summary>
    public interface IDelegationContextHandler : IContextHandler
    {
        /// <summary>
        /// Updates needed subject information for the Context Request for a specific delegation
        /// </summary>
        /// <param name="request">The original Xacml Context Request</param>
        /// <param name="subjects">The list of PartyIds to be added as subject attributes</param>
        public void Enrich(XacmlContextRequest request, List<int> subjects);

        /// <summary>
        /// Gets the value of the first found attribute matching the prioritized order of xacmlRequestAttributes provided, from the XacmlContextRequest subjects.
        /// </summary>
        /// <param name="request">The Xacml Context Request</param>
        /// <param name="xacmlRequestAttributeIds">Array of xacml request urn attribute identifiers to look for, in prioritized order. First found matching attribute is returned.</param>
        /// <returns>The value of the first found matching subject attribute if any exists</returns>
        public AttributeMatch GetSubjectAttributeMatch(XacmlContextRequest request, string[] xacmlRequestAttributeIds);

        /// <summary>
        /// Gets the user id from the XacmlContextRequest subject attribute
        /// </summary>
        /// <param name="request">The Xacml Context Request</param>
        /// <returns>The user id of the subject</returns>
        public int GetSubjectUserId(XacmlContextRequest request);

        /// <summary>
        /// Gets the party id from the XacmlContextRequest subject attribute
        /// </summary>
        /// <param name="request">The Xacml Context Request</param>
        /// <returns>The party id of the subject</returns>
        public int GetSubjectPartyId(XacmlContextRequest request);

        /// <summary>
        /// Gets a XacmlResourceAttributes model from the XacmlContextRequest
        /// </summary>
        /// <param name="request">The Xacml Context Request</param>
        /// <returns>XacmlResourceAttributes model</returns>
        public XacmlResourceAttributes GetResourceAttributes(XacmlContextRequest request);

        /// <summary>
        /// Gets a the Action string from the XacmlContextRequest
        /// </summary>
        /// <param name="request">The Xacml Context Request</param>
        /// <returns>Action attribute string value</returns>
        public string GetActionString(XacmlContextRequest request);

        /// <summary>
        /// Gets the list of mainunits for a subunit
        /// </summary>
        /// <param name="subUnitPartyId">The subunit partyId to check and retrieve mainunits for</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>List of mainunits</returns>
        public Task<List<MainUnit>> GetMainUnits(int subUnitPartyId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the list of keyrole unit partyIds for a user
        /// </summary>
        /// <param name="subjectUserId">The userid to retrieve keyrole unit for</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns>List of partyIds for units where user has keyrole</returns>
        public Task<List<int>> GetKeyRolePartyIds(int subjectUserId, CancellationToken cancellationToken = default);
    }
}
