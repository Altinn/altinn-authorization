using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global
namespace Altinn.Platform.Authorization.Functions;

/// <summary>
/// Function endpoint handling queue messages that failed multiple times
/// </summary>
public class DelegationEventsPoison
{
    private readonly ILogger<DelegationEventsPoison> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegationEventsPoison"/> class.
    /// </summary>
    /// <param name="logger">The logger</param>
    public DelegationEventsPoison(ILogger<DelegationEventsPoison> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Logs the failed message to Application Insights.
    /// </summary>
    /// <param name="queueMessage">The queue message.</param>
    [FunctionName(nameof(DelegationEventsPoison))]
    public void Run([QueueTrigger("delegationevents-poison", Connection = "QueueStorage")] QueueMessage queueMessage)
    {
        _logger.LogCritical(
            "Failed processing delegation queue item: id={id}, inserted={inserted}, expires={expires}, body={body}",
            queueMessage.MessageId,
            queueMessage.InsertedOn,
            queueMessage.ExpiresOn,
            queueMessage.Body.ToString());
    }
}
