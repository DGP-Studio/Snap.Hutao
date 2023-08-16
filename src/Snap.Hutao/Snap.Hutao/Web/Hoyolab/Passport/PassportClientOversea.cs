// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 通行证客户端 XRPC 版
/// </summary>
[ConstructorGenerated]
[HttpClient(HttpClientConfiguration.XRpc3)]
internal sealed partial class PassportClientOversea : IPassportClient
{
    private readonly ILogger<PassportClientOversea> logger;
    private readonly JsonSerializerOptions options;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步获取 CookieToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>cookie token</returns>
    [ApiInformation(Cookie = CookieType.SToken)]
    public async ValueTask<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default)
    {
        string? stoken = user.SToken?.GetValueOrDefault(Cookie.STOKEN);
        ArgumentException.ThrowIfNullOrEmpty(stoken);
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);
        STokenWrapper data = new(stoken, user.Aid);

        Response<UidCookieToken>? resp = await httpClient
            .SetUser(user, CookieType.SToken)
            .TryCatchPostAsJsonAsync<STokenWrapper, Response<UidCookieToken>>(ApiOsEndpoints.AccountGetCookieTokenBySToken, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取 LToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>uid 与 cookie token</returns>
    [ApiInformation(Cookie = CookieType.SToken)]
    public async ValueTask<Response<LTokenWrapper>> GetLTokenBySTokenAsync(User user, CancellationToken token = default)
    {
        string? stoken = user.SToken?.GetValueOrDefault(Cookie.STOKEN);
        ArgumentException.ThrowIfNullOrEmpty(stoken);
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);
        STokenWrapper data = new(stoken, user.Aid);

        Response<LTokenWrapper>? resp = await httpClient
            .SetUser(user, CookieType.SToken)
            .TryCatchPostAsJsonAsync<STokenWrapper, Response<LTokenWrapper>>(ApiOsEndpoints.AccountGetLTokenBySToken, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}