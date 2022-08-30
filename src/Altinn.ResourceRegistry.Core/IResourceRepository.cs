using Altinn.ResourceRegistry.Models;

namespace Altinn.ResourceRegistry.Core
{
    public interface IResourceRepository
    {

        ServiceResource GetResource(string id);

    }
}