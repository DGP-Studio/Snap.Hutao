// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Request;
using Snap.Hutao.Web.Response;
using System.Collections.Generic;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// 用户游戏角色提供器
/// </summary>
[Injection(InjectAs.Transient)]
internal class UserGameRoleProvider
{
    private readonly IUserService userService;
    private readonly Requester requester;

    /// <summary>
    /// 构造一个新的用户游戏角色提供器
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="requester">请求器</param>
    public UserGameRoleProvider(IUserService userService, Requester requester)
    {
        this.userService = userService;
        this.requester = requester;
    }

    /// <summary>
    /// 获取用户角色信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户角色信息</returns>
    public async Task<List<UserGameRole>> GetUserGameRolesAsync(CancellationToken cancellationToken = default)
    {
        Response<ListWrapper<UserGameRole>>? resp = await requester
            .Reset()
            .SetAcceptJson()
            .SetCommonUA()
            .SetRequestWithHyperion()
            .SetUser(userService.CurrentUser)
            .GetAsync<ListWrapper<UserGameRole>>(ApiEndpoints.UserGameRoles, cancellationToken)
            .ConfigureAwait(false);
        return resp?.Data?.List ?? new();
    }
}
