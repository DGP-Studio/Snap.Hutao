// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 通行证客户端
/// </summary>
internal interface IPassportClient : IOverseaSupport
{
    /// <summary>
    /// 异步获取 CookieToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>cookie token</returns>
    Task<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default);

    /// <summary>
    /// 异步获取 LToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>uid 与 cookie token</returns>
    Task<Response<LTokenWrapper>> GetLTokenBySTokenAsync(User user, CancellationToken token = default);
}