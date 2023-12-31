// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;

namespace Snap.Hutao.Web.Request.Builder;

[Serializable]
internal sealed class HttpContentSerializationException : Exception
{
    public HttpContentSerializationException(string? message, Exception? innerException)
        : base(message ?? GetDefaultMessage(), innerException)
    {
    }

    private HttpContentSerializationException(Exception? innerException)
        : base(GetDefaultMessage(), innerException)
    {
    }

    public static async ValueTask<HttpContentSerializationException> CreateAsync(HttpContent? content, Exception? innerException)
    {
        if (content is null)
        {
            return new(innerException);
        }

        string contentString = await content.ReadAsStringAsync().ConfigureAwait(false);
        string message = $"""
            The (de-)serialization failed because of an arbitrary error. This most likely happened, 
            because an inner serializer failed to (de-)serialize the given data. 
            ----- data begin -----
            {contentString}
            -----  data end  -----
            See the inner exception for details (if available).
            """;

        return new(message, innerException);
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