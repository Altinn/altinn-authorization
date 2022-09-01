using Altinn.AccessGroups.Core;
using Altinn.AccessGroups.Core.Models;
using Altinn.AccessGroups.Interfaces;
using Altinn.AccessGroups.Persistance;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

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

        public async Task<List<AccessGroup>> GetAccessGroups()
        {
            return await _accessGroupRepository.GetAccessGroups();
        }

        public async Task<ExternalRelationship> CreateExternalRelationship(ExternalRelationship externalRelationship)
        {
            if (externalRelationship.AccessGroupId == 0)
            {
                List<AccessGroup> accessGroups = await GetAccessGroups();
                AccessGroup accessGroup = accessGroups.FirstOrDefault(ag => ag.AccessGroupCode == externalRelationship.AccessGroupCode);
                if (accessGroup != null)
                {
                    externalRelationship.AccessGroupId = accessGroup.AccessGroupId;
                }
                else
                {
                    return externalRelationship;
                }               
            }

            return await _accessGroupRepository.InsertExternalRelationship(externalRelationship);
        }

        public async Task<List<ExternalRelationship>> ImportExternalRelationships(List<ExternalRelationship> externalRelationships)
        {
            List<ExternalRelationship> results = new();
            foreach (ExternalRelationship externalRelationship in externalRelationships)
            {
                results.Add(await CreateExternalRelationship(externalRelationship));
            }

            return results;
        }

        public async Task<List<ExternalRelationship>> GetExternalRelationships()
        {
            return await _accessGroupRepository.GetExternalRelationships();
        }

        public Task<bool> UpdateGroup(AccessGroup accessGroup)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AccessGroup>> ImportAccessGroups(List<AccessGroup> accessGroups)
        {
            List<AccessGroup> result = new();
            foreach (AccessGroup accessGroup in accessGroups)
            {
                result.Add(await _accessGroupRepository.InsertAccessGroup(accessGroup));
            }

            return result;
        }
    }
}
