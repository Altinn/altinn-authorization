﻿using Altinn.ResourceRegistry.Core;
using Altinn.ResourceRegistry.Core.Models;
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
            string resourcePath = GetResourcePath(id);
            if (File.Exists(resourcePath))
            {
                string content = System.IO.File.ReadAllText(resourcePath);
                ServiceResource? resource = System.Text.Json.JsonSerializer.Deserialize<ServiceResource>(content, new System.Text.Json.JsonSerializerOptions() {  PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase}) as ServiceResource;
                return resource;
            }

            return null;
        }

        public async Task<List<ServiceResource>> Search(ResourceSearch resourceSearch)
        {
            List<ServiceResource> resources = new List<ServiceResource>();
            string[] files =  Directory.GetFiles(GetResourcePath());
            if(files != null)
            {
                foreach (string file in files)
                {
                    string content = System.IO.File.ReadAllText(file);
                    ServiceResource? resource = System.Text.Json.JsonSerializer.Deserialize<ServiceResource>(content, new System.Text.Json.JsonSerializerOptions() { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase }) as ServiceResource;
                    resources.Add(resource);
                }
            }

            return resources;
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
