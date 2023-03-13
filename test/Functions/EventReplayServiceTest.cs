using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Functions.Clients.Interfaces;
using Altinn.Platform.Authorization.Functions.Exceptions;
using Altinn.Platform.Authorization.Functions.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Altinn.Platform.Authorization.Functions.UnitTest;

public class EventReplayServiceTest
{
    private readonly Mock<ILogger<EventReplayService>> _logger;
    private readonly Mock<IAuthorizationClient> _authorizationClient;

    public EventReplayServiceTest()
    {
        _logger = new Mock<ILogger<EventReplayService>>();
        _authorizationClient = new Mock<IAuthorizationClient>();

        _logger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
    }

    [Fact]
    public async Task PostDelegationEventsReplayAsync_Success()
    {
        // Arrange with 200 response
        EventReplayService eventReplayService = new EventReplayService(_logger.Object, _authorizationClient.Object);
        _authorizationClient.Setup(x => x.PostDelegationEventsReplayAsync(100000, 100000))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act and assert no exceptions thrown
        await eventReplayService.ReplayEvents(100000, 100000);
    }

    [Fact]
    public async Task PostDelegationEventsReplayAsync_InvalidStartId_Fail()
    {
        // Arrange with 400 response on invalid startId
        EventReplayService eventReplayService = new EventReplayService(_logger.Object, _authorizationClient.Object);
        HttpResponseMessage mockedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
        mockedResponse.Content = new StringContent("{\r\n    \"type\": \"https://tools.ietf.org/html/rfc7231#section-6.5.1\",\r\n    \"title\": \"One or more validation errors occurred.\",\r\n    \"status\": 400,\r\n    \"traceId\": \"00-889c13d346e47d985a8a29614d943a4f-e082010ab0bdf6e0-00\",\r\n    \"errors\": {\r\n        \"startId\": [\r\n            \"Must specify a valid starting delegation change id for replay. Invalid value: 0\"\r\n        ]\r\n    }\r\n}");
        _authorizationClient.Setup(x => x.PostDelegationEventsReplayAsync(0, 0))
            .ReturnsAsync(mockedResponse);

        // Act and assert no exceptions thrown
        await Assert.ThrowsAsync<AuthorizationRequestFailedException>(
          async () => await eventReplayService.ReplayEvents(0, 0));
    }

    [Fact]
    public async Task PostDelegationEventsReplayAsync_InvalidEndId_Fail()
    {
        // Arrange with 400 response on invalid endId
        EventReplayService eventReplayService = new EventReplayService(_logger.Object, _authorizationClient.Object);
        HttpResponseMessage mockedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
        mockedResponse.Content = new StringContent("{\r\n    \"type\": \"https://tools.ietf.org/html/rfc7231#section-6.5.1\",\r\n    \"title\": \"One or more validation errors occurred.\",\r\n    \"status\": 400,\r\n    \"traceId\": \"00-3cd99bc5400eeadda62079b1d818a6be-faae633e375ada31-00\",\r\n    \"errors\": {\r\n        \"endId\": [\r\n            \"The endId cannot be smaller than the startId. startId: 100000, endId: 10\"\r\n        ]\r\n    }\r\n}");
        _authorizationClient.Setup(x => x.PostDelegationEventsReplayAsync(100000, 10))
            .ReturnsAsync(mockedResponse);

        // Act and assert no exceptions thrown
        await Assert.ThrowsAsync<AuthorizationRequestFailedException>(
          async () => await eventReplayService.ReplayEvents(100000, 10));
    }

    [Fact]
    public async Task PostDelegationEventsReplayAsync_Unprocessable_Fail()
    {
        // Arrange with 422 unprocessable response on when no events found for replay matching the event id range
        EventReplayService eventReplayService = new EventReplayService(_logger.Object, _authorizationClient.Object);
        HttpResponseMessage mockedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
        mockedResponse.Content = new StringContent("{\r\n    \"type\": \"https://tools.ietf.org/html/rfc4918#section-11.2\",\r\n    \"title\": \"Unprocessable Entity\",\r\n    \"status\": 422,\r\n    \"traceId\": \"00-d2c235356b243f0c3a8fe3dac322f047-eb53387749934637-00\"\r\n}");
        _authorizationClient.Setup(x => x.PostDelegationEventsReplayAsync(10, 10))
            .ReturnsAsync(mockedResponse);

        // Act and assert no exceptions thrown
        await Assert.ThrowsAsync<AuthorizationRequestFailedException>(
          async () => await eventReplayService.ReplayEvents(10, 10));
    }

    [Fact]
    public async Task PostDelegationEventsReplayAsync_HttpRequestException_Fail()
    {
        // Arrange with 422 unprocessable response on when no events found for replay matching the event id range
        EventReplayService eventReplayService = new EventReplayService(_logger.Object, _authorizationClient.Object);
        _authorizationClient.Setup(x => x.PostDelegationEventsReplayAsync(10, 10))
            .ThrowsAsync(new HttpRequestException());

        // Act and assert no exceptions thrown
        await Assert.ThrowsAsync<HttpRequestException>(
          async () => await eventReplayService.ReplayEvents(10, 10));
    }
}
