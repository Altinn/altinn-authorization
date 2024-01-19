using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Interface;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Authorization.ABAC.Xacml.JsonProfile;
using Altinn.Common.Authentication.Configuration;
using Altinn.Platform.Authorization.Controllers;
using Altinn.Platform.Authorization.IntegrationTests.MockServices;
using Altinn.Platform.Authorization.IntegrationTests.Util;
using Altinn.Platform.Authorization.IntegrationTests.Webfactory;
using Altinn.Platform.Authorization.Repositories.Interface;
using Altinn.Platform.Authorization.Services.Interface;
using Altinn.Platform.Authorization.Services.Interfaces;
using Altinn.Platform.Events.Tests.Mocks;
using Altinn.ResourceRegistry.Tests.Mocks;
using AltinnCore.Authentication.JwtCookie;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Altinn.Platform.Authorization.IntegrationTests
{
    public class ExternalDecisionTest :IClassFixture<CustomWebApplicationFactory<DecisionController>>
    {
        private readonly CustomWebApplicationFactory<DecisionController> _factory;

        public ExternalDecisionTest(CustomWebApplicationFactory<DecisionController> fixture)
        {
            _factory = fixture;
        }

        [Fact]
        public async Task PDPExternal_Decision_AltinnApps0008()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");
            string testCase = "AltinnApps0008";
            HttpClient client = GetTestClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequestExternal(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDPExternal_Decision_AltinnResourceRegistry0005()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");
            string testCase = "AltinnResourceRegistry0005";
            HttpClient client = GetTestClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequestExternal(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDPExternal_Decision_AltinnResourceRegistry0006()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");

            string testCase = "AltinnResourceRegistry0006";
            HttpClient client = GetTestClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequestExternal(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDPExternal_Decision_AltinnResourceRegistry0007()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");
            string testCase = "AltinnResourceRegistry0007";
            HttpClient client = GetTestClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequestExternal(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDPExternal_Decision_AltinnResourceRegistry0008()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");
            string testCase = "AltinnResourceRegistry0008";
            HttpClient client = GetTestClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequestExternal(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        private HttpClient GetTestClient()
        {
            HttpClient client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IInstanceMetadataRepository, InstanceMetadataRepositoryMock>();
                    services.AddSingleton<IPolicyRetrievalPoint, PolicyRetrievalPointMock>();
                    services.AddSingleton<IDelegationMetadataRepository, DelegationMetadataRepositoryMock>();
                    services.AddSingleton<IRoles, RolesMock>();
                    services.AddSingleton<IOedRoleAssignmentWrapper, OedRoleAssignmentWrapperMock>();
                    services.AddSingleton<IParties, PartiesMock>();
                    services.AddSingleton<IProfile, ProfileMock>();
                    services.AddSingleton<IPolicyRepository, PolicyRepositoryMock>();
                    services.AddSingleton<IDelegationChangeEventQueue, DelegationChangeEventQueueMock>();
                    services.AddSingleton<IPostConfigureOptions<JwtCookieOptions>, JwtCookiePostConfigureOptionsStub>();
                    services.AddSingleton<IPostConfigureOptions<OidcProviderSettings>, OidcProviderPostConfigureSettingsStub>();
                    services.AddSingleton<IRegisterService, RegisterServiceMock>();
                });
            }).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            return client;
        }
    }
}
