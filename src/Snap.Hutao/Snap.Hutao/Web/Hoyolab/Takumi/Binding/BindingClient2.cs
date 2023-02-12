// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// Stoken绑定客户端
/// </summary>
[UseDynamicSecret]
[HttpClient(HttpClientConfigration.XRpc)]
internal class BindingClient2
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<BindingClient2> logger;

    /// <summary>
    /// 构造一个新的用户游戏角色提供器
    /// </summary>
    /// <param name="httpClient">请求器</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="logger">日志器</param>
    public BindingClient2(HttpClient httpClient, JsonSerializerOptions options, ILogger<BindingClient2> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 获取用户角色信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.Stoken, Salt = SaltType.LK2)]
    public async Task<List<UserGameRole>> GetUserGameRolesByStokenAsync(User user, CancellationToken token = default)
    {
        Response<ListWrapper<UserGameRole>>? resp = await httpClient
            .SetUser(user, CookieType.Stoken)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.LK2, true)
            .TryCatchGetFromJsonAsync<Response<ListWrapper<UserGameRole>>>(ApiEndpoints.UserGameRolesByStoken, options, logger, token)
            .ConfigureAwait(false);

        return EnumerableExtension.EmptyIfNull(resp?.Data?.List);
    }

    /// <summary>
    /// 异步生成祈愿验证密钥
    /// 需要stoken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="data">提交数据</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.Stoken, Salt = SaltType.LK2)]
    public async Task<Response<GameAuthKey>> GenerateAuthenticationKeyAsync(User user, GenAuthKeyData data, CancellationToken token = default)
    {
        Response<GameAuthKey>? resp = await httpClient
            .SetUser(user, CookieType.Stoken)
            .SetReferer(ApiEndpoints.AppMihoyoReferer)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.LK2, true)
            .TryCatchPostAsJsonAsync<GenAuthKeyData, Response<GameAuthKey>>(ApiEndpoints.BindingGenAuthKey, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}