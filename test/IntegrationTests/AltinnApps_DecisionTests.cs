using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Interface;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Authorization.ABAC.Xacml.JsonProfile;
using Altinn.Platform.Authorization.Clients.Interfaces;
using Altinn.Platform.Authorization.Controllers;
using Altinn.Platform.Authorization.IntegrationTests.MockServices;
using Altinn.Platform.Authorization.IntegrationTests.Util;
using Altinn.Platform.Authorization.IntegrationTests.Webfactory;
using Altinn.Platform.Authorization.Models.EventLog;
using Altinn.Platform.Authorization.Repositories.Interface;
using Altinn.Platform.Authorization.Services.Interface;
using Altinn.Platform.Authorization.Services.Interfaces;
using AltinnCore.Authentication.JwtCookie;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Moq;
using Xunit;

namespace Altinn.Platform.Authorization.IntegrationTests
{
    public class AltinnApps_DecisionTests :IClassFixture<CustomWebApplicationFactory<DecisionController>>
    {
        private readonly CustomWebApplicationFactory<DecisionController> _factory;
        private readonly Mock<IFeatureManager> featureManageMock = new Mock<IFeatureManager>();
        private readonly Mock<ISystemClock> systemClock = new Mock<ISystemClock>();

        public AltinnApps_DecisionTests(CustomWebApplicationFactory<DecisionController> fixture)
        {
            _factory = fixture;
            SetupFeatureMock(true);
            SetupDateTimeMock();
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0001()
        {
            string testCase = "AltinnApps0001";
            string bearerToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjM4OTJENDgyRTYyMDI2NzI1MTJBRTBDMkQ5REJBQzBERTRBNEVDMzciLCJ4NXQiOiJPSkxVZ3VZZ0puSlJLdURDMmR1c0RlU2s3RGMiLCJ0eXAiOiJKV1QifQ.eyJuYW1laWQiOiIyMDAxMDM3NyIsInVybjphbHRpbm46dXNlcmlkIjoiMjAwMTAzNzciLCJ1cm46YWx0aW5uOnBhcnR5aWQiOjUwOTExMTQzLCJ1cm46YWx0aW5uOmF1dGhlbnRpY2F0ZW1ldGhvZCI6IlZpcmtzb21oZXRzQnJ1a2VyIiwidXJuOmFsdGlubjphdXRobGV2ZWwiOjMsImp0aSI6IjA1ZWRiYzA0LWEzYWMtNDRhMy1hMjQyLTJjMWUzNTRkMzAzMiIsIm5iZiI6MTcwNzI5MzI0NiwiZXhwIjoxNzA3Mjk1MDQ2LCJpYXQiOjE3MDcyOTMyNDZ9.vuiYB8kqR0wXRjRuChlSpnPy4FDEyqRWSOf9tkNHNWaeyutpnmQCF8hnKodZd5vuwQTSW53FRqab0by4vN-W0YhU-NSDEQSlI75ING5uHKrw5l-D9uuA58jk35cr4cY_tcfamogAdISBBnRi5kqo7b8f_3yFfYVGlnhO3EYzRQl5gaJLi3OY9q8O6bdyPtJ78O_ksoYKiAT-N3WQqs0uHP6u5MXANKmVd7NRZiYaAPO2W7bS_KfMLolDLymX0hULjJc_DdAGfRhCaWxkPowdQToDKfL0vCgpHmPndwXQF7H_Yp7L9fiZIVr3ojwMNe0UI6-tI3LVf3pZ_c95u-eVCA";
            
            Mock<IFeatureManager> featureManageMock = new Mock<IFeatureManager>();
            featureManageMock
                .Setup(m => m.IsEnabledAsync("AuditLog"))
                .Returns(Task.FromResult(true));
            Mock<IEventsQueueClient> eventQueue = new Mock<IEventsQueueClient>();
            eventQueue.Setup(q => q.EnqueueAuthorizationEvent(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            AuthorizationEvent expectedAuthorizationEvent = TestSetupUtil.GetAuthorizationEvent(testCase);

            HttpClient client = GetTestClient(eventQueue.Object, featureManageMock.Object, systemClock.Object);
            client.DefaultRequestHeaders.Add("x-forwarded-for", "51.120.0.114, 10.122.16.225");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequest(testCase);
            XacmlContextResponse expected = TestSetupUtil.ReadExpectedResponse(testCase);

            // Act
            XacmlContextResponse contextResponse = await TestSetupUtil.GetXacmlContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
            AssertionUtil.AssertAuthorizationEvent(eventQueue, expectedAuthorizationEvent, Times.Once());
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0001_Auditlog_Off()
        {
            string testCase = "AltinnApps0001";

            Mock<IFeatureManager> featureManageMock = new Mock<IFeatureManager>();
            featureManageMock
                .Setup(m => m.IsEnabledAsync("AuditLog"))
                .Returns(Task.FromResult(false));

            Mock<IEventsQueueClient> eventQueue = new Mock<IEventsQueueClient>();
            eventQueue.Setup(q => q.EnqueueAuthorizationEvent(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            AuthorizationEvent expectedAuthorizationEvent = TestSetupUtil.GetAuthorizationEvent(testCase);

            HttpClient client = GetTestClient(eventQueue.Object, featureManageMock.Object);
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequest(testCase);
            XacmlContextResponse expected = TestSetupUtil.ReadExpectedResponse(testCase);

            // Act
            XacmlContextResponse contextResponse = await TestSetupUtil.GetXacmlContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
            AssertionUtil.AssertAuthorizationEvent(eventQueue, expectedAuthorizationEvent, Times.Never());
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0007()
        {
            string testCase = "AltinnApps0007";

            Mock<IFeatureManager> featureManageMock = new Mock<IFeatureManager>();
            featureManageMock
                .Setup(m => m.IsEnabledAsync("AuditLog"))
                .Returns(Task.FromResult(true));
            Mock<IEventsQueueClient> eventQueue = new Mock<IEventsQueueClient>();
            eventQueue.Setup(q => q.EnqueueAuthorizationEvent(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            AuthorizationEvent expectedAuthorizationEvent = TestSetupUtil.GetAuthorizationEvent(testCase);

            HttpClient client = GetTestClient(eventQueue.Object, featureManageMock.Object, systemClock.Object);            
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
            AssertionUtil.AssertAuthorizationEvent(eventQueue, expectedAuthorizationEvent, Times.Once());
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0002()
        {
            string testCase = "AltinnApps0002";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequest(testCase);
            XacmlContextResponse expected = TestSetupUtil.ReadExpectedResponse(testCase);

            // Act
            XacmlContextResponse contextResponse = await TestSetupUtil.GetXacmlContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0003()
        {
            string testCase = "AltinnApps0003";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequest(testCase);
            XacmlContextResponse expected = TestSetupUtil.ReadExpectedResponse(testCase);

            // Act
            XacmlContextResponse contextResponse = await TestSetupUtil.GetXacmlContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0004()
        {
            string testCase = "AltinnApps0004";

            Mock<IFeatureManager> featureManageMock = new Mock<IFeatureManager>();
            featureManageMock
                .Setup(m => m.IsEnabledAsync("AuditLog"))
                .Returns(Task.FromResult(true));
            Mock<IEventsQueueClient> eventQueue = new Mock<IEventsQueueClient>();
            eventQueue.Setup(q => q.EnqueueAuthorizationEvent(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            AuthorizationEvent expectedAuthorizationEvent = TestSetupUtil.GetAuthorizationEvent(testCase);

            HttpClient client = GetTestClient(eventQueue.Object, featureManageMock.Object, systemClock.Object);
            client.DefaultRequestHeaders.Add("x-forwarded-for", "51.120.0.114,20.251.13.24, 10.122.16.225");
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequest(testCase);
            XacmlContextResponse expected = TestSetupUtil.ReadExpectedResponse(testCase);

            // Act
            XacmlContextResponse contextResponse = await TestSetupUtil.GetXacmlContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
            AssertionUtil.AssertAuthorizationEvent(eventQueue, expectedAuthorizationEvent, Times.Once());
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0005()
        {
            string testCase = "AltinnApps0005";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequest(testCase);
            XacmlContextResponse expected = TestSetupUtil.ReadExpectedResponse(testCase);

            // Act
            XacmlContextResponse contextResponse = await TestSetupUtil.GetXacmlContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0006()
        {
            string testCase = "AltinnApps0006";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequest(testCase);
            XacmlContextResponse expected = TestSetupUtil.ReadExpectedResponse(testCase);

            // Act
            XacmlContextResponse contextResponse = await TestSetupUtil.GetXacmlContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0008()
        {
            string testCase = "AltinnApps0008";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0009()
        {
            string testCase = "AltinnApps0009";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0010()
        {
            string testCase = "AltinnApps0010";

            Mock<IFeatureManager> featureManageMock = new Mock<IFeatureManager>();
            featureManageMock
                .Setup(m => m.IsEnabledAsync("AuditLog"))
                .Returns(Task.FromResult(true));
            Mock<IEventsQueueClient> eventQueue = new Mock<IEventsQueueClient>();
            eventQueue.Setup(q => q.EnqueueAuthorizationEvent(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            AuthorizationEvent expectedAuthorizationEvent = null;

            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
            AssertionUtil.AssertAuthorizationEvent(eventQueue, expectedAuthorizationEvent, Times.Never());
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0011()
        {
            string testCase = "AltinnApps0011";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0012()
        {
            string testCase = "AltinnApps0012";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0027()
        {
            string testCase = "AltinnApps0027";
            HttpClient client = GetTestClient(null, featureManageMock.Object);
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps0001_Delegation()
        {
            string testCase = "AltinnApps0001Delegation";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequest(testCase);
            XacmlContextResponse expected = TestSetupUtil.ReadExpectedResponse(testCase);

            // Act
            XacmlContextResponse contextResponse = await TestSetupUtil.GetXacmlContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_DelegationPolicy_AltinnAppsOrg1App1_UserDelegation_Permit()
        {
            string testCase = "AltinnAppsOrg1App1_UserDelegation";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_DelegationPolicy_AltinnAppsOrg1App1_UserDelegation_MainUnit_Permit()
        {
            string testCase = "AltinnAppsOrg1App1_UserDelegation_MainUnit";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_DelegationPolicy_AltinnAppsOrg1App1_KeyRoleUnitDelegation_Permit()
        {
            string testCase = "AltinnAppsOrg1App1_KeyRoleUnitDelegation";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_DelegationPolicy_AltinnAppsOrg1App1_KeyRoleUnitDelegation_MainUnit_Permit()
        {
            string testCase = "AltinnAppsOrg1App1_KeyRoleUnitDelegation_MainUnit";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_DelegationPolicy_AltinnAppsOrg1App1_UserDelegation_Deleted_NotApplicable()
        {
            string testCase = "AltinnAppsOrg1App1_UserDelegation_Deleted";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps_OedFormuesfullmakt_Xml_Permit()
        {
            string testCase = "AltinnApps_OedFormuesfullmakt_Xml_Permit";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateXacmlRequest(testCase);
            XacmlContextResponse expected = TestSetupUtil.ReadExpectedResponse(testCase);

            // Act
            XacmlContextResponse contextResponse = await TestSetupUtil.GetXacmlContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        [Fact]
        public async Task PDP_Decision_AltinnApps_OedFormuesfullmakt_Json_Permit()
        {
            string testCase = "AltinnApps_OedFormuesfullmakt_Json_Permit";
            HttpClient client = GetTestClient();
            HttpRequestMessage httpRequestMessage = TestSetupUtil.CreateJsonProfileXacmlRequest(testCase);
            XacmlJsonResponse expected = TestSetupUtil.ReadExpectedJsonProfileResponse(testCase);

            // Act
            XacmlJsonResponse contextResponse = await TestSetupUtil.GetXacmlJsonProfileContextResponseAsync(client, httpRequestMessage);

            // Assert
            AssertionUtil.AssertEqual(expected, contextResponse);
        }

        private HttpClient GetTestClient(IEventsQueueClient eventLog = null, IFeatureManager featureManager = null, ISystemClock systemClockMock = null)
        {
            HttpClient client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IAccessManagementWrapper, AccessManagementWrapperMock>();
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
                    if (featureManager != null)
                    {
                        services.AddSingleton(featureManager);
                    }

                    if (eventLog != null)
                    {
                        services.AddSingleton(eventLog);
                    }

                    if (systemClockMock != null)
                    {
                        services.AddSingleton(systemClockMock);
                    }
                });
            }).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            return client;
        }

        private void SetupFeatureMock(bool featureFlag)
        {            
            featureManageMock
                .Setup(m => m.IsEnabledAsync("AuditLog"))
                .Returns(Task.FromResult(featureFlag));
        }

        private void SetupDateTimeMock()
        {
            systemClock
                .Setup(m => m.UtcNow)
                .Returns(new DateTimeOffset(2018, 05, 15, 02, 05, 00, new TimeSpan(1, 0, 0)));
        }
    }
}
