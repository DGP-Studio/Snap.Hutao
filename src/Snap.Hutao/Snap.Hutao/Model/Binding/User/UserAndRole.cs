// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Model.Binding.User;

/// <summary>
/// 角色与实体用户
/// </summary>
public class UserAndRole
{
    /// <summary>
    /// 构造一个新的实体用户与角色
    /// </summary>
    /// <param name="user">实体用户</param>
    /// <param name="role">角色</param>
    public UserAndRole(EntityUser user, UserGameRole role)
    {
        User = user;
        Role = role;
    }

    /// <summary>
    /// 实体用户
    /// </summary>
    public EntityUser User { get; private set; }

    /// <summary>
    /// 角色
    /// </summary>
    public UserGameRole Role { get; private set; }
}