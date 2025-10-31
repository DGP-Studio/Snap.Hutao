// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HoyoPlayClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IApiEndpointsFactory apiEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial HoyoPlayClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<GamePackagesWrapper>> GetPackagesAsync(LaunchScheme scheme, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(scheme.IsOversea).HoyoPlayConnectGamePackages(scheme))
            .Get();

        Response<GamePackagesWrapper>? resp = await builder
            .SendAsync<Response<GamePackagesWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<GameChannelSDKsWrapper>> GetChannelSDKAsync(LaunchScheme scheme, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(scheme.IsOversea).HoyoPlayConnectGameChannelSDKs(scheme))
            .Get();

        Response<GameChannelSDKsWrapper>? resp = await builder
            .SendAsync<Response<GameChannelSDKsWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<DeprecatedFileConfigurationsWrapper>> GetDeprecatedFileConfigurationsAsync(LaunchScheme scheme, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(scheme.IsOversea).HoyoPlayConnectDeprecatedFileConfigs(scheme))
            .Get();

        Response<DeprecatedFileConfigurationsWrapper>? resp = await builder
            .SendAsync<Response<DeprecatedFileConfigurationsWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<GameBranchesWrapper>> GetBranchesAsync(LaunchScheme scheme, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(scheme.IsOversea).HoyoPlayConnectGameBranches(scheme))
            .Get();

        Response<GameBranchesWrapper>? resp = await builder
            .SendAsync<Response<GameBranchesWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}