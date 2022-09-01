using Altinn.AccessGroups.Core.Models;
using Altinn.AccessGroups.Test;
using Altinn.AccessGroups.Test.Util;
using Altinn.AccessGroups.Controllers;
using System.Text.Json;
using System.Net;

namespace AccessGroupTest
{
    public class AccessGroupUnitTest : IClassFixture<CustomWebApplicationFactory<AccessGroupController>>
    {
        private readonly CustomWebApplicationFactory<AccessGroupController> _factory;

        public AccessGroupUnitTest(CustomWebApplicationFactory<AccessGroupController> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Test case: Calling the "Hello world" GET endpoint on the DelegationsController
        /// Expected: returns 200 OK with content: "Hello world!"
        /// </summary>
        [Fact]
        public async Task Get_HelloWorld()
        {
            // Act
            HttpClient client = SetupUtil.GetTestClient(_factory);
            string requestUri =  "authorization/api/v1/AccessGroup";
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri)
            {
            };

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
            string responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Hello world!", responseContent);
        }

        [Fact]
        public async Task ListGroupMembershipsTest()
        {
            HttpClient client = SetupUtil.GetTestClient(_factory);
            string requestUri = "authorization/api/v1/AccessGroup/ListGroupMemberships";

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {};

            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

            string responseContent = await response.Content.ReadAsStringAsync();
            List<AccessGroup>? resource = JsonSerializer.Deserialize<List<AccessGroup>>(responseContent, new System.Text.Json.JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) as List<AccessGroup>;

            Assert.NotNull(resource);
            //Assert.NotNull(resource.Identifier);

        }
    }
}