// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hoyolab.Hk4e.QrCode;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Globalization;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Account;

[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc2)]
internal sealed partial class SessionAppClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<SessionAppClient> logger;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 通过 GameToken 获取 SToken (V2)
    /// </summary>
    /// <param name="account">扫码获得的账户信息</param>
    /// <param name="token">取消令牌</param>
    /// <returns>登录结果</returns>
    public async ValueTask<Response<LoginResult>> PostSTokenByGameTokenAsync(QrCodeAccount account, CancellationToken token = default)
    {
        GameTokenWrapper wrapper = new()
        {
            Stuid = int.Parse(account.Stuid, CultureInfo.CurrentCulture),
            GameToken = account.GameToken,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.STokenByGameToken)
            .PostJson(wrapper);

        Response<LoginResult>? resp = await builder
            .TryCatchSendAsync<Response<LoginResult>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
