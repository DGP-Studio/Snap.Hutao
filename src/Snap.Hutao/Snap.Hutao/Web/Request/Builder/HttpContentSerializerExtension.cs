// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;
using System.Text;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpContentSerializerExtension
{
    public static HttpContent? Serialize<T>(this IHttpContentSerializer serializer, T content, Encoding? encoding)
    {
        ArgumentNullException.ThrowIfNull(serializer);
        return serializer.Serialize(content, typeof(T), encoding);
    }
}