// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;
using System.Text;

namespace Snap.Hutao.Web.Request.Builder;

internal abstract class HttpContentSerializer : IHttpContentSerializer, IHttpContentDeserializer
{
    protected internal virtual Encoding DefaultEncoding
    {
        get => Encoding.UTF8;
    }

    public HttpContent? Serialize(object? content, Type contentType, Encoding? encoding)
    {
        contentType = GetAndVerifyContentType(content, contentType);

        if (content is NoContent || contentType == typeof(NoContent))
        {
            return null;
        }

        try
        {
            return SerializeCore(content, contentType, encoding ?? DefaultEncoding);
        }
        catch (Exception ex) when (ex is not HttpContentSerializationException)
        {
            throw new HttpContentSerializationException(null, ex);
        }
    }

    public async ValueTask<object?> DeserializeAsync(HttpContent? httpContent, Type contentType, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(contentType);

        if (contentType == typeof(NoContent))
        {
            return default(NoContent);
        }

        try
        {
            return await DeserializeAsyncCore(httpContent, contentType, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not HttpContentSerializationException)
        {
            throw new HttpContentSerializationException(null, ex);
        }
    }

    protected abstract HttpContent? SerializeCore(object? content, Type contentType, Encoding encoding);

    protected abstract ValueTask<object?> DeserializeAsyncCore(HttpContent? httpContent, Type contentType, CancellationToken cancellationToken);

    private static Type GetAndVerifyContentType(object? content, Type contentType)
    {
        // If both types are given, ensure that they match. Otherwise serialization will be problematic.
        Type? actualContentType = content?.GetType();
        if (actualContentType is not null && contentType is not null && !contentType.IsAssignableFrom(actualContentType))
        {
            string message = $"""
                The content to be serialized does not match the specified type. 
                Expected an instance of the class "{contentType.FullName}", but got "{actualContentType.FullName}".
                """;
            ThrowHelper.Argument(message, nameof(contentType));
        }

        // The contentType is optional. In that case, try to get the type on our own.
        ArgumentNullException.ThrowIfNull(actualContentType);
        return contentType ?? actualContentType;
    }
}