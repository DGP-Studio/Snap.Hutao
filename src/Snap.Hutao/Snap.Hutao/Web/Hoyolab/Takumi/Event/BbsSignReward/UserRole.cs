// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// 将用户与角色捆绑在一起
/// </summary>
public struct UserRole
{
    /// <summary>
    /// 构造一个新的用户角色
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="role">角色</param>
    public UserRole(User user, UserGameRole role)
    {
        User = user;
        Role = role;
    }

    /// <summary>
    /// 用户
    /// </summary>
    public User User { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public UserGameRole Role { get; set; }
}