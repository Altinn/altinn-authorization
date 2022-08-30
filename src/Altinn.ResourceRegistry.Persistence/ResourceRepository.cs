using Altinn.ResourceRegistry.Core;
using Altinn.ResourceRegistry.Models;

namespace Altinn.ResourceRegistry.Persistence
{
    public class ResourceRepository : IResourceRepository

    {
        public ServiceResource GetResource(string id)
        {
            return new ServiceResource() { identifier = "test_harcoded_resourcerepostory" };
        }
    }
}