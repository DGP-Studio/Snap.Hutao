// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Sdk.Combo;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc2)]
internal sealed partial class PandaClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<PandaClient> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<UrlWrapper>> QRCodeFetchAsync(CancellationToken token = default)
    {
        // Use 12 (zzz) instead of 4 (gi) temporarily to get legacy game token
        GameLoginRequest options = GameLoginRequest.Create(12, HoyolabOptions.DeviceId40);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.QrCodeFetch)
            .SetHeader("x-rpc-device_id", HoyolabOptions.DeviceId40)
            .PostJson(options);

        Response<UrlWrapper>? resp = await builder
            .TryCatchSendAsync<Response<UrlWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<GameLoginResult>> QRCodeQueryAsync(string ticket, CancellationToken token = default)
    {
        GameLoginRequest options = GameLoginRequest.Create(12, HoyolabOptions.DeviceId40, ticket);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.QrCodeQuery)
            .SetHeader("x-rpc-device_id", HoyolabOptions.DeviceId40)
            .PostJson(options);

        Response<GameLoginResult>? resp = await builder
            .TryCatchSendAsync<Response<GameLoginResult>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}