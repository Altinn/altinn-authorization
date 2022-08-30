using Altinn.ResourceRegistry.Core;
using Altinn.ResourceRegistry.Models;

namespace Altinn.ResourceRegistry.Persistence
{
    public class ResourceRepository : IResourceRepository

    {
        public void CreateResource(ServiceResource resource)
        {
            throw new NotImplementedException();
        }

        public void DeleteResource(string id)
        {
            throw new NotImplementedException();
        }

        public ServiceResource GetResource(string id)
        {
            return new ServiceResource() { identifier = "test_harcoded_resourcerepostory" };
        }

        public void UpdateResource(ServiceResource resource)
        {
            throw new NotImplementedException();
        }
    }
}