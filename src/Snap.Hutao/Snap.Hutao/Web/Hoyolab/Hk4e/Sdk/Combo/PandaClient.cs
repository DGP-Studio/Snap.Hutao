// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Sdk.Combo;

[HttpClient(HttpClientConfiguration.XRpc2)]
internal sealed partial class PandaClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    [FromKeyed(ApiEndpointsKind.Chinese)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial PandaClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<UrlWrapper>> QRCodeFetchAsync(CancellationToken token = default)
    {
        GameLoginRequest options = GameLoginRequest.Create(8, HoyolabOptions.DeviceId40);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.QrCodeFetch())
            .PostJson(options);

        Response<UrlWrapper>? resp = await builder
            .SendAsync<Response<UrlWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<GameLoginResult>> QRCodeQueryAsync(string ticket, CancellationToken token = default)
    {
        GameLoginRequest options = GameLoginRequest.Create(8, HoyolabOptions.DeviceId40, ticket);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.QrCodeQuery())
            .SetHeader("x-rpc-device_id", HoyolabOptions.DeviceId40)
            .PostJson(options);

        Response<GameLoginResult>? resp = await builder
            .SendAsync<Response<GameLoginResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}