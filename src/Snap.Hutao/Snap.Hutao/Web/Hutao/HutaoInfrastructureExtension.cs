// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Snap.Hutao.Web.Hutao;

internal static class HutaoInfrastructureExtension
{
    public static async ValueTask InfrastructureSetTraceInfoAsync<TBuilder>(this TBuilder builder, HutaoUserOptions hutaoUserOptions)
        where TBuilder : IHttpHeadersBuilder<HttpRequestHeaders>
    {
        builder
            .SetHeader("x-homa-token", await hutaoUserOptions.GetAuthTokenAsync().ConfigureAwait(false));
    }

    public static ValueTask InfrastructureSetTraceInfoAsync(this HttpRequestMessage message, HutaoUserOptions hutaoUserOptions)
    {
        return new HttpRequestHeadersBuilder(message.Headers).InfrastructureSetTraceInfoAsync(hutaoUserOptions);
    }

    // Wraps HttpRequestHeaders to IHttpHeadersBuilder
    private sealed class HttpRequestHeadersBuilder : IHttpHeadersBuilder<HttpRequestHeaders>
    {
        public HttpRequestHeadersBuilder(HttpRequestHeaders headers)
        {
            Headers = headers;
        }

        public HttpRequestHeaders Headers { get; }
    }
}