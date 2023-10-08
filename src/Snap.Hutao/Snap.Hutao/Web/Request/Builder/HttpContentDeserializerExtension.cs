// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpContentDeserializerExtension
{
    public static async ValueTask<T?> DeserializeAsync<T>(this IHttpContentDeserializer deserializer, HttpContent? httpContent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(deserializer);
        return (T?)await deserializer.DeserializeAsync(httpContent, typeof(T), cancellationToken).ConfigureAwait(false);
    }
}