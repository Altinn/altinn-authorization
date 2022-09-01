using Altinn.AccessGroups.Core;
using Altinn.AccessGroups.Core.Models;
using Altinn.AccessGroups.Interfaces;
using Altinn.AccessGroups.Persistance;

namespace Altinn.AccessGroups.Services
{
    public class AccessGroupService : IAccessGroup
    {
        private readonly ILogger<IAccessGroup> _logger;
        private readonly IAccessGroupsRepository _accessGroupRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessGroupService"/> class.
        /// </summary>
        /// <param name="accessGroupRepository">The repository client for access groups</param>
        /// <param name="logger">Logger instance</param>
        public AccessGroupService(IAccessGroupsRepository accessGroupRepository, ILogger<IAccessGroup> logger)
        {
            _accessGroupRepository = accessGroupRepository;
            _logger = logger;
        }

        public async Task<AccessGroup> CreateGroup(AccessGroup accessGroup)
        {
            return await _accessGroupRepository.InsertAccessGroup(accessGroup);
        }

        public Task<List<AccessGroup>> ExportAccessGroups()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ImportAccessGroups(List<AccessGroup> accessGroups)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateGroup(AccessGroup accessGroup)
        {
            throw new NotImplementedException();
        }
    }
}
