// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.App.Account;

/// <summary>
/// 账户客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc)]
internal sealed partial class AccountClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<AccountClient> logger;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步生成米游社操作验证密钥
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="data">提交数据</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.SToken, Salt = SaltType.K2)]
    public async ValueTask<Response<GameAuthKey>> GenerateAuthenticationKeyAsync(User user, GenAuthKeyData data, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.AppAuthGenAuthKey)
            .SetUserCookie(user, CookieType.SToken)
            .SetReferer(ApiEndpoints.AppMihoyoReferer)
            .PostJson(data);

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen1, SaltType.K2, false).ConfigureAwait(false);

        Response<GameAuthKey>? resp = await builder
            .TryCatchSendAsync<Response<GameAuthKey>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
