using System;
using System.Runtime.Serialization;

namespace Altinn.Platform.Authorization.Functions.Exceptions;

/// <summary>
/// Generic exception used thrown on unexpected response from authorization API
/// </summary>
[Serializable]
public class AuthorizationRequestFailedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationRequestFailedException"/> class.
    /// </summary>
    public AuthorizationRequestFailedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationRequestFailedException"/> class.
    /// </summary>
    /// <param name="message">Error message</param>
    public AuthorizationRequestFailedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationRequestFailedException"/> class.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public AuthorizationRequestFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationRequestFailedException"/> class.
    /// </summary>
    /// <param name="info">Serialization info</param>
    /// <param name="context">Context</param>
    protected AuthorizationRequestFailedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
