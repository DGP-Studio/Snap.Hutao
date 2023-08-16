// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 通行证客户端 XRPC 版
/// </summary>
[HighQuality]
[UseDynamicSecret]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc2)]
internal sealed partial class PassportClient : IPassportClient
{
    private readonly ILogger<PassportClient2> logger;
    private readonly JsonSerializerOptions options;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步获取 CookieToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>cookie token</returns>
    [ApiInformation(Cookie = CookieType.SToken, Salt = SaltType.PROD)]
    public async ValueTask<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default)
    {
        Response<UidCookieToken>? resp = await httpClient
            .SetUser(user, CookieType.SToken)
            .UseDynamicSecret(DynamicSecretVersion.Gen2, SaltType.PROD, true)
            .TryCatchGetFromJsonAsync<Response<UidCookieToken>>(ApiEndpoints.AccountGetCookieTokenBySToken, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取 LToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>uid 与 cookie token</returns>
    [ApiInformation(Cookie = CookieType.SToken, Salt = SaltType.PROD)]
    public async ValueTask<Response<LTokenWrapper>> GetLTokenBySTokenAsync(User user, CancellationToken token = default)
    {
        Response<LTokenWrapper>? resp = await httpClient
            .SetUser(user, CookieType.SToken)
            .UseDynamicSecret(DynamicSecretVersion.Gen2, SaltType.PROD, true)
            .TryCatchGetFromJsonAsync<Response<LTokenWrapper>>(ApiEndpoints.AccountGetLTokenBySToken, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}