using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;

using Altinn.Authorization.ABAC.Xacml;
using Altinn.Authorization.ABAC.Xacml.JsonProfile;
using Altinn.Common.PEP.Configuration;
using Altinn.Common.PEP.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Altinn.Common.PEP.Authorization
{
    public class ResourceAccessHandlerTest
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IPDP> _pdpMock;
        private readonly IOptions<PepSettings> _generalSettings;
        private readonly ResourceAccessHandler _rah;

        public ResourceAccessHandlerTest()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _pdpMock = new Mock<IPDP>();
            _generalSettings = Options.Create(new PepSettings());
            _rah = new ResourceAccessHandler(_httpContextAccessorMock.Object, _pdpMock.Object, new Mock<ILogger<ResourceAccessHandler>>().Object);
        }

        /// <summary>
        /// Test case: Send request and get response that fulfills all requirements
        /// Expected: Context will succeed
        /// </summary>
        [Fact]
        public async Task HandleRequirementAsync_TC01Async()
        {
            // Arrange 
            AuthorizationHandlerContext context = CreateAuthorizationHandlerContext();
            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(CreateHttpContext("r23453546546"));
            XacmlJsonResponse response = CreateResponse(XacmlContextDecision.Permit.ToString());
            _pdpMock.Setup(a => a.GetDecisionForRequest(It.IsAny<XacmlJsonRequestRoot>())).Returns(Task.FromResult(response));

            // Act
            await _rah.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
            Assert.False(context.HasFailed);
        }

        /// <summary>
        /// Test case: Send request and get response that fulfills all requirements
        /// Expected: Context will succeed
        /// </summary>
        [Fact]
        public async Task HandleRequirementAsync_TC02Async()
        {
            // Arrange 
            AuthorizationHandlerContext context = CreateAuthorizationHandlerContext();
            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(CreateHttpContext("991825827"));
            XacmlJsonResponse response = CreateResponse(XacmlContextDecision.Permit.ToString());
            _pdpMock.Setup(a => a.GetDecisionForRequest(It.IsAny<XacmlJsonRequestRoot>())).Returns(Task.FromResult(response));

            // Act
            await _rah.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
            Assert.False(context.HasFailed);
        }

        /// <summary>
        /// Test case: Send request and get response that fulfills all requirements
        /// Expected: Exception since format of who is incorrect
        /// </summary>
        [Fact]

        public async Task HandleRequirementAsync_TC03Async()
        {
            // Arrange 
            AuthorizationHandlerContext context = CreateAuthorizationHandlerContext();
            _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(CreateHttpContext("991825827M"));
            XacmlJsonResponse response = CreateResponse(XacmlContextDecision.Permit.ToString());
            _pdpMock.Setup(a => a.GetDecisionForRequest(It.IsAny<XacmlJsonRequestRoot>())).Returns(Task.FromResult(response));

            // Act
            Task Act() => _rah.HandleAsync(context);

            // Assert
            ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(Act);
            Assert.Equal("invalid who - ssn used", exception.Message);
        }

        private ClaimsPrincipal CreateUser()
        {
            // Create the user
            List<Claim> claims = new List<Claim>();

            // type, value, valuetype, issuer
            claims.Add(new Claim("urn:name", "Ola", "string", "org"));
            claims.Add(new Claim("urn:altinn:authlevel", "2", "string", "org"));

            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            return user;
        }

        private HttpContext CreateHttpContext(string who)
        {
            HttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.RouteValues.Add("who", who);
            return httpContext;
        }

        private XacmlJsonResponse CreateResponse(string decision)
        {
            // Create response 
            XacmlJsonResponse response = new XacmlJsonResponse();
            response.Response = new List<XacmlJsonResult>();

            // Set result to premit
            XacmlJsonResult result = new XacmlJsonResult();
            result.Decision = decision;
            response.Response.Add(result);

            return response;
        }

        private XacmlJsonResponse AddObligationWithMinAuthLv(XacmlJsonResponse response, string minAuthLv)
        {
            // Add obligation to result with a minimum authentication level attribute
            XacmlJsonResult result = response.Response[0];
            XacmlJsonObligationOrAdvice obligation = new XacmlJsonObligationOrAdvice();
            obligation.AttributeAssignment = new List<XacmlJsonAttributeAssignment>();
            XacmlJsonAttributeAssignment authenticationAttribute = new XacmlJsonAttributeAssignment()
            {
                Category = "urn:altinn:minimum-authenticationlevel",
                Value = minAuthLv
            };
            obligation.AttributeAssignment.Add(authenticationAttribute);
            result.Obligations = new List<XacmlJsonObligationOrAdvice>();
            result.Obligations.Add(obligation);

            return response;
        }

        private AuthorizationHandlerContext CreateAuthorizationHandlerContext()
        {
            ResourceAccessRequirement requirement = new ResourceAccessRequirement("read", "altinn_access_management_apidelegation");
            ClaimsPrincipal user = CreateUser();
            Document resource = default(Document);
            AuthorizationHandlerContext context = new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                resource);
            return context;
        }
    }
}
