// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 用户信息客户端
/// </summary>
internal interface IUserClient : IOverseaSupport
{
    /// <summary>
    /// 获取当前用户详细信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>详细信息</returns>
    Task<Response<UserFullInfoWrapper>> GetUserFullInfoAsync(Model.Entity.User user, CancellationToken token = default);
}
