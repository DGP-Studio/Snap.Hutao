// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 通行证客户端 XRPC 版
/// </summary>
[UseDynamicSecret]
[HttpClient(HttpClientConfigration.XRpc2)]
internal class PassportClient2
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<PassportClient> logger;

    /// <summary>
    /// 构造一个新的通行证客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">json序列化选项</param>
    /// <param name="logger">日志器</param>
    public PassportClient2(HttpClient httpClient, JsonSerializerOptions options, ILogger<PassportClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// V1Stoken 登录
    /// </summary>
    /// <param name="stokenV1">v1 Stoken</param>
    /// <param name="token">取消令牌</param>
    /// <returns>登录数据</returns>
    [ApiInformation(Salt = SaltType.PROD)]
    public async Task<Response<LoginResult>> LoginByStokenAsync(Cookie stokenV1, CancellationToken token)
    {
        HttpResponseMessage message = await httpClient
            .SetHeader("Cookie", stokenV1.ToString())
            .UseDynamicSecret(DynamicSecretVersion.Gen2, SaltType.PROD, true)
            .PostAsync(ApiEndpoints.AccountGetSTokenByOldToken, null, token)
            .ConfigureAwait(false);

        Response<LoginResult>? resp = await message.Content
            .ReadFromJsonAsync<Response<LoginResult>>(options, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取 CookieToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>cookie token</returns>
    [ApiInformation(Cookie = CookieType.Stoken, Salt = SaltType.PROD)]
    public async Task<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default)
    {
        Response<UidCookieToken>? resp = await httpClient
            .SetUser(user, CookieType.Stoken)
            .UseDynamicSecret(DynamicSecretVersion.Gen2, SaltType.PROD, true)
            .TryCatchGetFromJsonAsync<Response<UidCookieToken>>(ApiEndpoints.AccountGetCookieTokenBySToken, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取 Ltoken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>uid 与 cookie token</returns>
    [ApiInformation(Cookie = CookieType.Stoken, Salt = SaltType.PROD)]
    public async Task<Response<LtokenWrapper>> GetLtokenBySTokenAsync(User user, CancellationToken token)
    {
        Response<LtokenWrapper>? resp = await httpClient
            .SetUser(user, CookieType.Stoken)
            .UseDynamicSecret(DynamicSecretVersion.Gen2, SaltType.PROD, true)
            .TryCatchGetFromJsonAsync<Response<LtokenWrapper>>(ApiEndpoints.AccountGetLtokenByStoken, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}