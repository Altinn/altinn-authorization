using System;
using System.Runtime.Serialization;

namespace Altinn.Platform.Authorization.Functions.Exceptions;

/// <summary>
/// Generic exception used to trigger re-queueing of messages
/// </summary>
[Serializable]
public class BridgeRequestFailedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BridgeRequestFailedException"/> class.
    /// </summary>
    public BridgeRequestFailedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BridgeRequestFailedException"/> class.
    /// </summary>
    /// <param name="message">Error message</param>
    public BridgeRequestFailedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BridgeRequestFailedException"/> class.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public BridgeRequestFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BridgeRequestFailedException"/> class.
    /// </summary>
    /// <param name="info">Serialization info</param>
    /// <param name="context">Context</param>
    protected BridgeRequestFailedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
