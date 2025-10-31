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

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class GachaInfoClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IApiEndpointsFactory apiEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial GachaInfoClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<GachaLogPage>> GetGachaLogPageAsync(GachaLogTypedQueryOptions options, CancellationToken token = default)
    {
        string query = options.ToQueryString();

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(options.IsOversea).GachaInfoGetGachaLog(query))
            .Get();

        Response<GachaLogPage>? resp = await builder
            .SendAsync<Response<GachaLogPage>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}