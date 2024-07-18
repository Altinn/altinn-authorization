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
        public async Task PDPExternal_Decision_AltinnApps0010()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");
            string testCase = "AltinnApps0010";
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

        /// <summary>
        /// Scenario where org is listed in policy. Should work. Policy org is digir. Authz subject org is digdir
        /// </summary>
        [Fact]
        public async Task PDPExternal_Decision_AltinnResourceRegistry0009()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");
            string testCase = "AltinnResourceRegistry0009";
            HttpClient client = GetTestClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequestExternal(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        /// <summary>
        /// Scenario where org is listed in policy. Should not work. Policy org is digir. Authz subject org is nav
        /// </summary>
        [Fact]
        public async Task PDPExternal_Decision_AltinnResourceRegistry0010()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");
            string testCase = "AltinnResourceRegistry0010";
            HttpClient client = GetTestClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequestExternal(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        /// <summary>
        /// Scenario where org is listed in policy. Should work. Policy org is digir. Authz subject org is digdir. Uses old attribute id
        /// </summary>
        [Fact]
        public async Task PDPExternal_Decision_AltinnResourceRegistry0011()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");
            string testCase = "AltinnResourceRegistry0011";
            HttpClient client = GetTestClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequestExternal(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        /// <summary>
        /// Scenario where systemuser has received delegation from the resource party for the resource. Should give Permit result.
        /// </summary>
        [Fact]
        public async Task PDPExternal_Decision_SystemUserWithResourceDelegation_Permit()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");
            string testCase = "ResourceRegistry_SystemUserWithDelegation_Permit";
            HttpClient client = GetTestClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequestExternal(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        /// <summary>
        /// Scenario where systemuser has received delegation from the resource party for the resource. Should give Permit result.
        /// </summary>
        [Fact]
        public async Task PDPExternal_Decision_SystemUserWithAppDelegation_Permit()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");
            string testCase = "AltinnApps_SystemUserWithDelegation_Permit";
            HttpClient client = GetTestClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequestExternal(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        /// <summary>
        /// Scenario where systemuser has received delegation, but request includes multiple subjects as org and orgnumber. Should NOT give Permit. 
        /// </summary>
        [Fact]
        public async Task PDPExternal_Decision_SystemUserWithDelegation_TooManyRequestSubjects_Indeterminate()
        {
            string token = PrincipalUtil.GetOrgToken("skd", "974761076", "altinn:authorization:pdp");
            string testCase = "ResourceRegistry_SystemUserWithDelegation_TooManyRequestSubjects_Indeterminate";
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
                    services.AddSingleton<IAccessManagementWrapper, AccessManagementWrapperMock>();
                });
            }).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            return client;
        }
    }
}
