// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class GachaInfoClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IApiEndpointsFactory apiEndpointsFactory;
    private readonly ILogger<GachaInfoClient> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<GachaLogPage>> GetGachaLogPageAsync(GachaLogTypedQueryOptions options, CancellationToken token = default)
    {
        string query = options.ToQueryString();

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(options.IsOversea).GachaInfoGetGachaLog(query))
            .Get();

        Response<GachaLogPage>? resp = await builder
            .SendAsync<Response<GachaLogPage>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}