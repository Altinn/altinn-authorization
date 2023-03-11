using System;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Functions.Models;
using Altinn.Platform.Authorization.Functions.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global
namespace Altinn.Platform.Authorization.Functions;

/// <summary>
/// Function endpoint for triggering replay of delegation events
/// </summary>
public class DelegationEventsReplay
{
    private readonly IEventReplayService _eventReplayService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegationEventsReplay"/> class.
    /// </summary>
    public DelegationEventsReplay(IEventReplayService eventReplayService)
    {
        _eventReplayService = eventReplayService;
    }

    /// <summary>
    /// Function endpoint for replay of delegation events
    /// </summary>
    [FunctionName(nameof(DelegationEventsReplay))]
    public async Task RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        int startId;
        int endId = 0;
        string startIdParam = req.Query["startId"];
        string endIdParam = req.Query["endId"];

        if (startIdParam == null || !int.TryParse(startIdParam, out startId))
        {
            throw new ArgumentException("Must specify a valid integer for 'startId' query param for the delegation change id range to replay");
        }

        if (endIdParam != null && !int.TryParse(endIdParam, out endId))
        {
            throw new ArgumentException("'endId' query param is not a valid integer");
        }

        await _eventReplayService.ReplayEvents(startId, endId);
    }
}
