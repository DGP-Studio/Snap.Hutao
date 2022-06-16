// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Request;
using Snap.Hutao.Web.Response;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// 用户游戏角色提供器
/// </summary>
[Injection(InjectAs.Transient)]
internal class UserGameRoleClient
{
    private readonly IUserService userService;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 构造一个新的用户游戏角色提供器
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="httpClient">请求器</param>
    public UserGameRoleClient(IUserService userService, HttpClient httpClient)
    {
        this.userService = userService;
        this.httpClient = httpClient;
    }

    /// <summary>
    /// 获取用户角色信息
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    public async Task<List<UserGameRole>> GetUserGameRolesAsync(CancellationToken token = default)
    {
        Response<ListWrapper<UserGameRole>>? resp = await httpClient
            .SetUser(userService.CurrentUser)
            .GetFromJsonAsync<Response<ListWrapper<UserGameRole>>>(ApiEndpoints.UserGameRoles, token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data?.List);
    }
}
