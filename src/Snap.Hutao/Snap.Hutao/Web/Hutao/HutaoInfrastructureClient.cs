// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
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
    private readonly RuntimeOptions runtimeOptions;
    private readonly HttpClient httpClient;

    public async ValueTask<HutaoResponse<StaticResourceSizeInformation>> GetStaticSizeAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.StaticSize())
            .Get();

        HutaoResponse<StaticResourceSizeInformation>? resp = await builder.SendAsync<HutaoResponse<StaticResourceSizeInformation>>(httpClient, logger, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<IPInformation>> GetIPInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.Ip())
            .Get();

        HutaoResponse<IPInformation>? resp = await builder.SendAsync<HutaoResponse<IPInformation>>(httpClient, logger, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<HutaoPackageInformation>> GetHutaoVersionInfomationAsync(CancellationToken token = default)
    {
        string url = HutaoEndpoints.PatchSnapHutao();

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .SetHeader("x-hutao-device-id", runtimeOptions.DeviceId)
            .Get();

        HutaoResponse<HutaoPackageInformation>? resp = await builder.SendAsync<HutaoResponse<HutaoPackageInformation>>(httpClient, logger, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<YaeVersionInformation>> GetYaeVersionInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.PatchYaeAchievement())
            .Get();

        HutaoResponse<YaeVersionInformation>? resp = await builder.SendAsync<HutaoResponse<YaeVersionInformation>>(httpClient, logger, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }
}