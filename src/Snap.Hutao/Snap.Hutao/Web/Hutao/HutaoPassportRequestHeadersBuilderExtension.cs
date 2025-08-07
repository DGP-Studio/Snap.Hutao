// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http.Headers;

namespace Snap.Hutao.Web.Hutao;

internal static class HutaoPassportRequestHeadersBuilderExtension
{
    public static TBuilder SetAccessToken<TBuilder>(this TBuilder builder, string? accessToken)
        where TBuilder : IHttpHeadersBuilder<HttpRequestHeaders>
    {
        builder.Headers.Authorization = string.IsNullOrEmpty(accessToken) ? default : new("Bearer", accessToken);
        return builder;
    }

    public static TBuilder SetHomaToken<TBuilder>(this TBuilder builder, string? homaToken)
        where TBuilder : IHttpHeadersBuilder<HttpRequestHeaders>
    {
        if (!string.IsNullOrEmpty(homaToken))
        {
            builder.Headers.Add("x-homa-token", homaToken);
        }

        return builder;
    }
}