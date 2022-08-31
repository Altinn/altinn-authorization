﻿using Altinn.ResourceRegistry.Core.Models;
using Altinn.ResourceRegistry.Models;

namespace Altinn.ResourceRegistry.Core
{
    public interface IResourceRegistryRepository
    {
        Task<ServiceResource> GetResource(string id);

        Task DeleteResource(string id);

        Task UpdateResource(ServiceResource resource);

        Task CreateResource(ServiceResource resource);

        Task<List<ServiceResource>> Search(ResourceSearch resourceSearch);
    }
}