// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.App.Account;

/// <summary>
/// 账户客户端
/// </summary>
[HighQuality]
[UseDynamicSecret]
[HttpClient(HttpClientConfigration.XRpc)]
internal sealed class AccountClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<AccountClient> logger;

    /// <summary>
    /// 构造一个新的账户客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">选项</param>
    /// <param name="logger">日志器</param>
    public AccountClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<AccountClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步生成米游社操作验证密钥
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="data">提交数据</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.Stoken, Salt = SaltType.K2)]
    public async Task<Response<GameAuthKey>> GenerateAuthenticationKeyAsync(User user, GenAuthKeyData data, CancellationToken token = default)
    {
        Response<GameAuthKey>? resp = await httpClient
            .SetUser(user, CookieType.Stoken)
            .SetReferer(ApiEndpoints.AppMihoyoReferer)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.K2, false)
            .TryCatchPostAsJsonAsync<GenAuthKeyData, Response<GameAuthKey>>(ApiEndpoints.AppAuthGenAuthKey, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
