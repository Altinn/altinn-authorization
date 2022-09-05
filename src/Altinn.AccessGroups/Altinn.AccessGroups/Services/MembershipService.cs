using Altinn.AccessGroups.Core;
using Altinn.AccessGroups.Core.Models;
using Altinn.AccessGroups.Interfaces;
using Authorization.Platform.Authorization.Models;
using System.Linq;

namespace Altinn.AccessGroups.Services
{
    public class MembershipService : IMemberships
    {
        private readonly ILogger<IMemberships> _logger;
        private readonly IAccessGroupsRepository _accessGroupRepository;
        private readonly IAccessGroup _accessGroups;
        private readonly IAltinnRolesClient _altinnRoles;

        /// <summary>
        /// Initializes a new instance of the <see cref="MembershipService"/> class.
        /// </summary>
        /// <param name="accessGroupRepository">The repository client for access groups</param>
        /// <param name="accessGroup">The access group service implementation</param>
        /// <param name="altinnRoles">The Altinn role integration client</param>
        /// <param name="logger">Logger instance</param>
        public MembershipService(IAccessGroupsRepository accessGroupRepository, IAccessGroup accessGroups, IAltinnRolesClient altinnRoles, ILogger<IMemberships> logger)
        {
            _accessGroupRepository = accessGroupRepository;
            _accessGroups = accessGroups;
            _altinnRoles = altinnRoles;
            _logger = logger;
        }

        public Task<GroupMembership> ListMemberships()
        {
            Task<GroupMembership> result = _accessGroupRepository.ListGroupmemberships();
            return result;
        }

        public Task<bool> AddMembership(GroupMembership input)
        {
            Task<bool> result = _accessGroupRepository.InsertGroupMembership(input);
            return result;
        }

        public async Task<List<AccessGroup>> ListGroupMemberships(AccessGroupSearch search)
        {
            if(!search.CoveredByUserId.HasValue)
            {
                throw new NotImplementedException();
            }

            List<Role> erRoles = await _altinnRoles.GetDecisionPointRolesForUser((int)search.CoveredByUserId, search.OfferedByPartyId);

            List<ExternalRelationship> externalRelationships = await _accessGroups.GetExternalRelationships();

            List<AccessGroup> accessGroups = await _accessGroups.GetAccessGroups();

            List<AccessGroup> result = new();
            result.AddRange(erRoles.SelectMany(role => externalRelationships.SelectMany(rel => accessGroups.Where(ag => ag.AccessGroupId == rel.AccessGroupId && rel.ExternalId == role.Value))));
            return result;
        }
        
        public async Task<bool> RevokeMembership(GroupMembership input)
        {
            throw new NotImplementedException();
        }
    }
}
