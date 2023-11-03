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

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// SToken绑定客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class BindingClient2
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<BindingClient2> logger;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 获取用户角色信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.SToken, Salt = SaltType.LK2)]
    public async ValueTask<Response<ListWrapper<UserGameRole>>> GetUserGameRolesBySTokenAsync(User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.AccountGetCookieTokenBySToken)
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .SetReferer(ApiEndpoints.AppMihoyoReferer)
            .Get();

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen1, SaltType.LK2, true).ConfigureAwait(false);

        Response<ListWrapper<UserGameRole>>? resp = await builder
            .TryCatchSendAsync<Response<ListWrapper<UserGameRole>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步生成祈愿验证密钥
    /// 需要 SToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="data">提交数据</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.SToken, Salt = SaltType.LK2)]
    public async ValueTask<Response<GameAuthKey>> GenerateAuthenticationKeyAsync(User user, GenAuthKeyData data, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.BindingGenAuthKey)
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .SetReferer(ApiEndpoints.AppMihoyoReferer)
            .PostJson(data);

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen1, SaltType.LK2, true).ConfigureAwait(false);

        Response<GameAuthKey>? resp = await builder
            .TryCatchSendAsync<Response<GameAuthKey>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}