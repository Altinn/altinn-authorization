using Altinn.ResourceRegistry.Models;

namespace Altinn.ResourceRegistry.Core
{
    public interface IResourceRepository
    {
        ServiceResource GetResource(string id);

        void DeleteResource(string id);

        void UpdateResource(ServiceResource resource);

        void CreateResource(ServiceResource resource);
    }
}