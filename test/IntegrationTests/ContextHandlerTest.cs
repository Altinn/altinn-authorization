using System.Threading.Tasks;
using Altinn.ApiClients.Maskinporten.Config;
using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Services;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Configuration;
using Altinn.Platform.Authorization.IntegrationTests.MockServices;
using Altinn.Platform.Authorization.IntegrationTests.Util;
using Altinn.Platform.Authorization.Services.Implementation;
using Altinn.Platform.Events.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Moq;
using Xunit;

namespace Altinn.Platform.Authorization.IntegrationTests
{
    /// <summary>
    /// Test class for <see cref="ContextHandler"></see>
    /// </summary>
    public class ContextHandlerTest 
    {
        private readonly ContextHandler _contextHandler;
        private HttpContext _httpContext = new DefaultHttpContext();

        public ContextHandlerTest()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(h => h.HttpContext).Returns(_httpContext);
            _contextHandler = new ContextHandler(
                new InstanceMetadataRepositoryMock(),
                new RolesMock(),
                new PartiesMock(),
                new MemoryCache(new MemoryCacheOptions()),
                Options.Create(new GeneralSettings { RoleCacheTimeout = 5 }),
                new RegisterServiceMock(),
                new PolicyRetrievalPointMock(httpContextAccessorMock.Object, new Logger<PolicyRetrievalPointMock>(new LoggerFactory())),
                new OedRoleAssignmentWrapperMock());

            // new OedRoleAssignmentWrapper(new System.Net.Http.HttpClient(), Options.Create(new GeneralSettings { }), new MaskinportenService(new System.Net.Http.HttpClient(), new Logger<MaskinportenService>(new LoggerFactory()), new MemoryTokenCacheProvider(new MemoryCache(Options.Create(new MemoryCacheOptions())))), Options.Create(new MaskinportenSettings())));
        }

        /// <summary>
        /// Scenario:
        /// Tests if the xacml request is enriched with the required resource, subject attributes
        /// Input:
        /// Instance id, user id, action
        /// Expected Result:
        /// Xacml request is enriched with the missing resource, roles and subject attributes
        /// Success Criteria:
        /// A xacml request populated with the required attributes is returned
        /// </summary>
        [Fact]
        public async Task ContextHandler_TC01()
        {
            // Arrange
            string testCase = "AltinnApps0021";
            _httpContext.Request.Headers.Add("testcase", testCase);

            XacmlContextRequest request = TestSetupUtil.CreateXacmlContextRequest(testCase);
            XacmlContextRequest expectedEnrichedRequest = TestSetupUtil.GetEnrichedRequest(testCase);

            // Act
            XacmlContextRequest enrichedRequest = await _contextHandler.Enrich(request);

            // Assert
            Assert.NotNull(enrichedRequest);
            Assert.NotNull(expectedEnrichedRequest);
            AssertionUtil.AssertEqual(expectedEnrichedRequest, enrichedRequest);
        }

        /// <summary>
        /// Scenario:
        /// Tests if the xacml request is enriched with the required resource, subject attributes
        /// Input:
        /// Instance id, org, action
        /// Expected Result:
        /// Xacml request is enriched with the missing resource and subject attributes
        /// Success Criteria:
        /// A xacml request populated with the required attributes is returned
        /// </summary>
        [Fact]
        public async Task ContextHandler_TC02()
        {
            // Arrange
            string testCase = "AltinnApps0022";
            _httpContext.Request.Headers.Add("testcase", testCase);

            XacmlContextRequest request = TestSetupUtil.CreateXacmlContextRequest(testCase);
            XacmlContextRequest expectedEnrichedRequest = TestSetupUtil.GetEnrichedRequest(testCase);

            // Act
            XacmlContextRequest enrichedRequest = await _contextHandler.Enrich(request);

            // Assert
            Assert.NotNull(enrichedRequest);
            Assert.NotNull(expectedEnrichedRequest);
            AssertionUtil.AssertEqual(expectedEnrichedRequest, enrichedRequest);
        }

        /// <summary>
        /// Scenario:
        /// Tests if the xacml request is enriched with the required resource, subject attributes
        /// Input:
        /// Complete resource attributes
        /// Expected Result:
        /// Xacml request is enriched with the missing role attributes
        /// Success Criteria:
        /// A xacml request populated with the required attributes is returned
        /// </summary>
        [Fact]
        public async Task ContextHandler_TC03()
        {
            // Arrange
            string testCase = "AltinnApps0023";
            _httpContext.Request.Headers.Add("testcase", testCase);

            XacmlContextRequest request = TestSetupUtil.CreateXacmlContextRequest(testCase);
            XacmlContextRequest expectedEnrichedRequest = TestSetupUtil.GetEnrichedRequest(testCase);

            // Act
            XacmlContextRequest enrichedRequest = await _contextHandler.Enrich(request);

            // Assert
            Assert.NotNull(enrichedRequest);
            Assert.NotNull(expectedEnrichedRequest);
            AssertionUtil.AssertEqual(expectedEnrichedRequest, enrichedRequest);
        }

        /// <summary>
        /// Scenario:
        /// Tests if the xacml request is enriched with the required resource, subject attributes
        /// Input:
        /// org, app, userid, partyid, action
        /// Expected Result:
        /// Xacml request is enriched with the missing role attributes
        /// Success Criteria:
        /// A xacml request populated with the required attributes is returned
        /// </summary>
        [Fact]
        public async Task ContextHandler_TC04()
        {
            // Arrange
            string testCase = "AltinnApps0024";
            _httpContext.Request.Headers.Add("testcase", testCase);

            XacmlContextRequest request = TestSetupUtil.CreateXacmlContextRequest(testCase);
            XacmlContextRequest expectedEnrichedRequest = TestSetupUtil.GetEnrichedRequest(testCase);

            // Act
            XacmlContextRequest enrichedRequest = await _contextHandler.Enrich(request);

            // Assert
            Assert.NotNull(enrichedRequest);
            Assert.NotNull(expectedEnrichedRequest);
            AssertionUtil.AssertEqual(expectedEnrichedRequest, enrichedRequest);
        }

        /// <summary>
        /// Scenario:
        /// Tests if the xacml request is enriched with the required resource, subject attributes
        /// Input:
        /// org, app, party id, action
        /// Expected Result:
        /// Xacml request is enriched with the missing role attributes
        /// Success Criteria:
        /// A xacml request populated with the required attributes is returned
        /// </summary>
        [Fact]
        public async Task ContextHandler_TC05()
        {
            // Arrange
            string testCase = "AltinnApps0025";
            _httpContext.Request.Headers.Add("testcase", testCase);

            XacmlContextRequest request = TestSetupUtil.CreateXacmlContextRequest(testCase);
            XacmlContextRequest expectedEnrichedRequest = TestSetupUtil.GetEnrichedRequest(testCase);

            // Act
            XacmlContextRequest enrichedRequest = await _contextHandler.Enrich(request);

            // Assert
            Assert.NotNull(enrichedRequest);
            Assert.NotNull(expectedEnrichedRequest);
            AssertionUtil.AssertEqual(expectedEnrichedRequest, enrichedRequest);
        }

        /// <summary>
        /// Scenario:
        /// Tests if the xacml request is enriched with the required resource, subject attributes
        /// Input:
        /// Instance-id, user-id, party-id
        /// Expected Result:
        /// Xacml request is enriched with the missing attributes
        /// Success Criteria:
        /// A xacml request populated with the required attributes is returned
        /// </summary>
        [Fact]
        public async Task ContextHandler_TC06()
        {
            // Arrange
            string testCase = "AltinnApps0026";
            _httpContext.Request.Headers.Add("testcase", testCase);

            XacmlContextRequest request = TestSetupUtil.CreateXacmlContextRequest(testCase);
            XacmlContextRequest expectedEnrichedRequest = TestSetupUtil.GetEnrichedRequest(testCase);

            // Act
            XacmlContextRequest enrichedRequest = await _contextHandler.Enrich(request);

            // Assert
            Assert.NotNull(enrichedRequest);
            Assert.NotNull(expectedEnrichedRequest);
            AssertionUtil.AssertEqual(expectedEnrichedRequest, enrichedRequest);
        }

        /// <summary>
        /// Scenario:
        /// Tests if the xacml request is enriched with the required rolecode
        /// Input:
        /// subject's ssn, resource's id, resource's ssn
        /// Expected Result:
        /// Xacml request is enriched with the missing attributes
        /// Success Criteria:
        /// A xacml request populated with the required attributes is returned
        /// </summary>
        [Fact]
        public async Task EnrichWithOedRoleUsingSubjectSsn()
        {
            // Arrange
            string testCase = "AltinnAppsOedSsn";
            _httpContext.Request.Headers.Add("testcase", testCase);

            XacmlContextRequest request = TestSetupUtil.CreateXacmlContextRequest(testCase);
            XacmlContextRequest expectedEnrichedRequest = TestSetupUtil.GetEnrichedRequest(testCase);

            // Act
            XacmlContextRequest enrichedRequest = await _contextHandler.Enrich(request);

            // Assert
            Assert.NotNull(enrichedRequest);
            Assert.NotNull(expectedEnrichedRequest);
            AssertionUtil.AssertEqual(expectedEnrichedRequest, enrichedRequest);
        }

        /// <summary>
        /// Scenario:
        /// Tests if the xacml request is enriched with the required rolecode
        /// Input:
        /// subject's Altinn role, resource's id, resource's ssn
        /// Expected Result:
        /// Xacml request is enriched with the missing attributes
        /// Success Criteria:
        /// A xacml request populated with the required attributes is returned
        /// </summary>
        [Fact]
        public async Task EnrichWithOedRoleUsingSubjecUserId()
        {
            // Arrange
            string testCase = "AltinnAppsOedUserId";
            _httpContext.Request.Headers.Add("testcase", testCase);

            XacmlContextRequest request = TestSetupUtil.CreateXacmlContextRequest(testCase);
            XacmlContextRequest expectedEnrichedRequest = TestSetupUtil.GetEnrichedRequest(testCase);

            // Act
            XacmlContextRequest enrichedRequest = await _contextHandler.Enrich(request);

            // Assert
            Assert.NotNull(enrichedRequest);
            Assert.NotNull(expectedEnrichedRequest);
            AssertionUtil.AssertEqual(expectedEnrichedRequest, enrichedRequest);
        }
    }
}
