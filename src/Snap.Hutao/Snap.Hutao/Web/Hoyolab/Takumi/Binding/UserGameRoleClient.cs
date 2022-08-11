// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Web.Response;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// 用户游戏角色提供器
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class UserGameRoleClient
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// 构造一个新的用户游戏角色提供器
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="httpClient">请求器</param>
    public UserGameRoleClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <summary>
    /// 获取用户角色信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    public async Task<List<UserGameRole>> GetUserGameRolesAsync(User user, CancellationToken token = default)
    {
        Response<ListWrapper<UserGameRole>>? resp = await httpClient
            .SetUser(user)
            .GetFromJsonAsync<Response<ListWrapper<UserGameRole>>>(ApiEndpoints.UserGameRoles, token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data?.List);
    }
}
