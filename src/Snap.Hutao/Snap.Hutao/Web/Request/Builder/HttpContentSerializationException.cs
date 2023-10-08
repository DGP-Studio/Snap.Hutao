// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Request.Builder;

[Serializable]
internal sealed class HttpContentSerializationException : Exception
{
    public HttpContentSerializationException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException)
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