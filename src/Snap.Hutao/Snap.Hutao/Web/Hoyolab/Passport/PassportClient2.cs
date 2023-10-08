// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 通行证客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc2)]
internal sealed partial class PassportClient2
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<PassportClient2> logger;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步验证 LToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>验证信息</returns>
    [ApiInformation(Cookie = CookieType.LToken)]
    public async ValueTask<Response<UserInfoWrapper>> VerifyLtokenAsync(User user, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.AccountVerifyLtoken)
            .SetUserCookie(user, CookieType.LToken)
            .PostJson(new Timestamp());

        Response<UserInfoWrapper>? resp = await builder
            .TryCatchSendAsync<Response<UserInfoWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// V1 SToken 登录
    /// </summary>
    /// <param name="stokenV1">v1 SToken</param>
    /// <param name="token">取消令牌</param>
    /// <returns>登录数据</returns>
    [ApiInformation(Salt = SaltType.PROD)]
    public async ValueTask<Response<LoginResult>> LoginBySTokenAsync(Cookie stokenV1, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.AccountGetSTokenByOldToken)
            .SetHeader("Cookie", stokenV1.ToString())
            .Post();

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.PROD, true).ConfigureAwait(false);

        Response<LoginResult>? resp = await builder
            .TryCatchSendAsync<Response<LoginResult>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    private class Timestamp
    {
        [JsonPropertyName("t")]
        public long Time { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}