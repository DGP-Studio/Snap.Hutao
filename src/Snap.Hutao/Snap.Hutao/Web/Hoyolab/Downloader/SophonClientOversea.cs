// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Downloader;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class SophonClientOversea : ISophonClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<SophonClientOversea> logger;
    [FromKeyed(ApiEndpointsKind.Oversea)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<SophonBuild>> GetBuildAsync(BranchWrapper branch, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.SophonChunkGetBuildByBranch(branch))
            .Get();

        Response<SophonBuild>? resp = await builder
            .SendAsync<Response<SophonBuild>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
