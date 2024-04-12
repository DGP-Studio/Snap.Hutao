// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao;

[HttpClient(HttpClientConfiguration.Default)]
[ConstructorGenerated(ResolveHttpClient = true)]
internal sealed partial class HutaoInfrastructureClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<HutaoInfrastructureClient> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<HutaoResponse<StaticResourceSizeInformation>> GetStaticSizeAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.StaticSize)
            .Get();

        HutaoResponse<StaticResourceSizeInformation>? resp = await builder.TryCatchSendAsync<HutaoResponse<StaticResourceSizeInformation>>(httpClient, logger, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<IPInformation>> GetIPInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.Ip)
            .Get();

        HutaoResponse<IPInformation>? resp = await builder.TryCatchSendAsync<HutaoResponse<IPInformation>>(httpClient, logger, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<HutaoVersionInformation>> GetHutaoVersionInfomationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.PatchSnapHutao)
            .Get();

        HutaoResponse<HutaoVersionInformation>? resp = await builder.TryCatchSendAsync<HutaoResponse<HutaoVersionInformation>>(httpClient, logger, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<YaeVersionInformation>> GetYaeVersionInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.PatchYaeAchievement)
            .Get();

        HutaoResponse<YaeVersionInformation>? resp = await builder.TryCatchSendAsync<HutaoResponse<YaeVersionInformation>>(httpClient, logger, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }
}