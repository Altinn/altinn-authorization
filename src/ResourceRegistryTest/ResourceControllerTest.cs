using Altinn.ResourceRegistry.Models;
using ResourceRegistry.Controllers;
using ResourceRegistryTest.Utils;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace ResourceRegistryTest
{
    public class ResourceControllerTest : IClassFixture<CustomWebApplicationFactory<ResourceController>>
    {

        private readonly CustomWebApplicationFactory<ResourceController> _factory;

        public ResourceControllerTest(CustomWebApplicationFactory<ResourceController> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test1Async_Get()
        {
            HttpClient client = SetupUtil.GetTestClient(_factory);
            string requestUri = "ResourceRegistry/api/Resource/altinn_access_management";

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri)
            {
            };

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            string responseContent = await response.Content.ReadAsStringAsync();
            ServiceResource? resource = System.Text.Json.JsonSerializer.Deserialize<ServiceResource>(responseContent, new System.Text.Json.JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) as ServiceResource;

            Assert.NotNull(resource);
            Assert.NotNull(resource.Identifier);

        }

        [Fact]
        public async Task Search_Get()
        {
            HttpClient client = SetupUtil.GetTestClient(_factory);
            string requestUri = "/api/Resource/Search?SearchTerm=test";

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri)
            {
            };

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            string responseContent = await response.Content.ReadAsStringAsync();
            List<ServiceResource>? resource = System.Text.Json.JsonSerializer.Deserialize<List<ServiceResource>>(responseContent, new System.Text.Json.JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) as List<ServiceResource>;

            Assert.NotNull(resource);
            Assert.Equal(2, resource.Count);

        }
    }
}