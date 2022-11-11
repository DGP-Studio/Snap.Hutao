// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// 绑定客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class BindingClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<BindingClient> logger;

    /// <summary>
    /// 构造一个新的用户游戏角色提供器
    /// </summary>
    /// <param name="httpClient">请求器</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="logger">日志器</param>
    public BindingClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<BindingClient> logger)
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
    public Task<List<UserGameRole>> GetUserGameRolesAsync(User user, CancellationToken token = default)
    {
        return GetUserGameRolesAsync(user.Entity, token);
    }

    /// <summary>
    /// 获取用户角色信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    public async Task<List<UserGameRole>> GetUserGameRolesAsync(Model.Entity.User user, CancellationToken token = default)
    {
        Response<ListWrapper<UserGameRole>>? resp = await httpClient
            .SetUser(user)
            .TryCatchGetFromJsonAsync<Response<ListWrapper<UserGameRole>>>(ApiEndpoints.UserGameRoles, options, logger, token)
            .ConfigureAwait(false);

        return EnumerableExtension.EmptyIfNull(resp?.Data?.List);
    }
}
