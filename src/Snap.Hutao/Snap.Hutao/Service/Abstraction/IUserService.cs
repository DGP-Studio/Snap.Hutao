// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using System.Collections.Generic;

namespace Snap.Hutao.Service.Abstraction;

/// <summary>
/// 用户服务
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    User Current { get; }

    /// <summary>
    /// 获取用户信息枚举
    /// 每个用户信息都应准备完成
    /// </summary>
    /// <returns>准备完成的用户信息枚举</returns>
    Task<IEnumerable<User>> GetInitializedUsersAsync();
}