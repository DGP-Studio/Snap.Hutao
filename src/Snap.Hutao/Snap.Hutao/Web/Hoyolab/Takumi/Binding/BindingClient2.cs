// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// SToken绑定客户端
/// </summary>
[HighQuality]
[UseDynamicSecret]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc)]
internal sealed partial class BindingClient2
{
    private readonly ILogger<BindingClient2> logger;
    private readonly JsonSerializerOptions options;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 获取用户角色信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.SToken, Salt = SaltType.LK2)]
    public async Task<List<UserGameRole>> GetUserGameRolesBySTokenAsync(User user, CancellationToken token = default)
    {
        Response<ListWrapper<UserGameRole>>? resp = await httpClient
            .SetUser(user, CookieType.SToken)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.LK2, true)
            .TryCatchGetFromJsonAsync<Response<ListWrapper<UserGameRole>>>(ApiEndpoints.UserGameRolesBySToken, options, logger, token)
            .ConfigureAwait(false);

        return EnumerableExtension.EmptyIfNull(resp?.Data?.List);
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
    public async Task<Response<GameAuthKey>> GenerateAuthenticationKeyAsync(User user, GenAuthKeyData data, CancellationToken token = default)
    {
        Response<GameAuthKey>? resp = await httpClient
            .SetUser(user, CookieType.SToken)
            .SetReferer(ApiEndpoints.AppMihoyoReferer)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.LK2, true)
            .TryCatchPostAsJsonAsync<GenAuthKeyData, Response<GameAuthKey>>(ApiEndpoints.BindingGenAuthKey, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}