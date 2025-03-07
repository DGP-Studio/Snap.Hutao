// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.Net.Http;
using System.Net.Mime;

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

        HttpContentSerializationException exception = new(GetDefaultMessage(), innerException);

        // Cache the content in array, in case the response disposed.
        ByteAttachmentContent attachmentContent = new(await content.ReadAsByteArrayAsync().ConfigureAwait(false));
        SentryAttachment attachment = new(AttachmentType.Default, attachmentContent, "content.txt", MediaTypeNames.Text.Plain);
        ExceptionAttachment.SetAttachment(exception, attachment);

        return exception;
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