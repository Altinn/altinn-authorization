using Altinn.ResourceRegistry.Core;
using Altinn.ResourceRegistry.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceRegistryTest.Mocks
{
    public class RegisterResourceRepositoryMock : IResourceRegistryRepository
    {
        public Task CreateResource(ServiceResource resource)
        {
            throw new NotImplementedException();
        }

        public Task DeleteResource(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResource> GetResource(string id)
        {
            string evPath = GetResourcePath(id);
            if (File.Exists(evPath))
            {
                string content = System.IO.File.ReadAllText(evPath);
                ServiceResource? resource = System.Text.Json.JsonSerializer.Deserialize<ServiceResource>(content) as ServiceResource;
                return resource;
            }

            return null;
        }

        private static string GetResourcePath(string id)
        {
            return Path.Combine(GetResourcePath(), id + ".json");
        }

        public Task UpdateResource(ServiceResource resource)
        {
            throw new NotImplementedException();
        }

        private static string GetResourcePath()
        {
            string unitTestFolder = Path.GetDirectoryName(new Uri(typeof(RegisterResourceRepositoryMock).Assembly.Location).LocalPath);
            return Path.Combine(unitTestFolder, @"..\..\..\Data\Resources");
        }
    }
}
