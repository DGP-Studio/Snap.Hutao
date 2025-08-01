// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Win32;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao;

[HttpClient(HttpClientConfiguration.Default)]
[ConstructorGenerated(ResolveHttpClient = true)]
internal sealed partial class HutaoInfrastructureClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly HttpClient httpClient;

    public async ValueTask<HutaoResponse<StaticResourceSizeInformation>> GetStaticSizeAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(StaticResourcesEndpoints.StaticSize())
            .Get();

        HutaoResponse<StaticResourceSizeInformation>? resp = await builder.SendAsync<HutaoResponse<StaticResourceSizeInformation>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<IPInformation>> GetIPInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().Ip())
            .Get();

        HutaoResponse<IPInformation>? resp = await builder.SendAsync<HutaoResponse<IPInformation>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<HutaoPackageInformation>> GetHutaoVersionInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PatchSnapHutao())
            .Get();

        HutaoResponse<HutaoPackageInformation>? resp = await builder.SendAsync<HutaoResponse<HutaoPackageInformation>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<YaeVersionInformation>> GetYaeVersionInformationAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PatchYaeAchievement())
            .Get();

        HutaoResponse<YaeVersionInformation>? resp = await builder.SendAsync<HutaoResponse<YaeVersionInformation>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse> AmIBannedAsync(string uid, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().AmIBanned())
            .SetHeader("x-hutao-island-identifier", HutaoNative.Instance.ExchangeGameUidForIdentifier1820(uid))
            .Get();

        HutaoResponse? resp = await builder.SendAsync<HutaoResponse>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }
}