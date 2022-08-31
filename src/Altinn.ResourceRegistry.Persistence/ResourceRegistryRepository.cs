using Altinn.ResourceRegistry.Core;
using Altinn.ResourceRegistry.Core.Models;
using Altinn.ResourceRegistry.Models;

namespace Altinn.ResourceRegistry.Persistence
{
    public class ResourceRepository : IResourceRegistryRepository
    {
        public Task<List<ServiceResource>> Search(ResourceSearch resourceSearch)
        {
            throw new NotImplementedException();
        }

        Task IResourceRegistryRepository.CreateResource(ServiceResource resource)
        {
            throw new NotImplementedException();
        }

        Task IResourceRegistryRepository.DeleteResource(string id)
        {
            throw new NotImplementedException();
        }

        Task<ServiceResource> IResourceRegistryRepository.GetResource(string id)
        {
            throw new NotImplementedException();
        }

        Task IResourceRegistryRepository.UpdateResource(ServiceResource resource)
        {
            throw new NotImplementedException();
        }
    }
}