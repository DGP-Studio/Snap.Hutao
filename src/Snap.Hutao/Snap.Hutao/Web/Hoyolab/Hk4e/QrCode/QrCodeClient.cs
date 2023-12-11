// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.QrCode;

[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class QrCodeClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<QrCodeClient> logger;
    private readonly HttpClient httpClient;

    private readonly string device = Core.Random.GetLetterAndNumberString(64);

    /// <summary>
    /// 异步获取扫码链接
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>login url</returns>
    public async ValueTask<Response<QrCodeFetch>> PostQrCodeFetchAsync(CancellationToken token = default)
    {
        QrCodeFetchOptions options = new(4, device);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.QrCodeFetch)
            .PostJson(options);

        Response<QrCodeFetch>? resp = await builder
            .TryCatchSendAsync<Response<QrCodeFetch>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取扫码状态
    /// </summary>
    /// <param name="ticket">扫码链接中的ticket</param>
    /// <param name="token">取消令牌/param>
    /// <returns>扫码状态</returns>
    public async ValueTask<Response<QrCodeQuery>> PostQrCodeQueryAsync(string ticket, CancellationToken token = default)
    {
        QrCodeQueryOptions options = new(4, device, ticket);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.QrCodeQuery)
            .PostJson(options);

        Response<QrCodeQuery>? resp = await builder
            .TryCatchSendAsync<Response<QrCodeQuery>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
