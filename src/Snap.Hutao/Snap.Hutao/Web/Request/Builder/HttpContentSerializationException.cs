// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.Serialization;

namespace Snap.Hutao.Web.Request.Builder;

[Serializable]
public class HttpContentSerializationException : Exception
{
    public HttpContentSerializationException()
        : this(message: null, innerException: null)
    {
    }

    public HttpContentSerializationException(string? message)
        : this(message, innerException: null)
    {
    }

    public HttpContentSerializationException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException)
    {
    }

    protected HttpContentSerializationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    private static string GetDefaultMessage()
    {
        return """
            The (de-)serialization failed because of an arbitrary error. This most likely happened, 
            because an inner serializer failed to (de-)serialize the given data. 
            See the inner exception for details (if available).
            """;
    }
}