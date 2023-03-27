// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Net.Http.Json;
using Windows.ApplicationModel.Contacts;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 通行证客户端 XRPC 版
/// </summary>
[HttpClient(HttpClientConfiguration.XRpc3, typeof(IPassportClient))]
internal sealed class PassportClientOversea : IPassportClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<PassportClientOversea> logger;

    /// <summary>
    /// 构造一个新的国际服通行证客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="logger">日志器</param>
    public PassportClientOversea(HttpClient httpClient, JsonSerializerOptions options, ILogger<PassportClientOversea> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public bool IsOversea => true;

    /// <summary>
    /// 异步获取 CookieToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>cookie token</returns>
    [ApiInformation(Cookie = CookieType.SToken)]
    public async Task<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default)
    {
        STokenWrapper data = new(user.SToken?.GetValueOrDefault(Cookie.STOKEN)!, user.Aid!);

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
    public async Task<Response<LTokenWrapper>> GetLTokenBySTokenAsync(User user, CancellationToken token = default)
    {
        STokenWrapper data = new(user.SToken?.GetValueOrDefault(Cookie.STOKEN)!, user.Aid!);

        Response<LTokenWrapper>? resp = await httpClient
            .SetUser(user, CookieType.SToken)
            .TryCatchPostAsJsonAsync<STokenWrapper, Response<LTokenWrapper>>(ApiOsEndpoints.AccountGetLTokenBySToken, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}