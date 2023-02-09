// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Model.Binding.User;

/// <summary>
/// 实体用户与角色
/// 由于许多操作需要同时用到ck与uid
/// 抽象此类用于简化这类调用
/// </summary>
public class UserAndUid
{
    /// <summary>
    /// 构造一个新的实体用户与角色
    /// </summary>
    /// <param name="user">实体用户</param>
    /// <param name="role">角色</param>
    public UserAndUid(EntityUser user, PlayerUid role)
    {
        User = user;
        Uid = role;
    }

    /// <summary>
    /// 实体用户
    /// </summary>
    public EntityUser User { get; private set; }

    /// <summary>
    /// 角色
    /// </summary>
    public PlayerUid Uid { get; private set; }

    /// <summary>
    /// 尝试转换到用户与角色
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="userAndUid">用户与角色</param>
    /// <returns>是否转换成功</returns>
    public static bool TryFromUser(User? user, [NotNullWhen(true)] out UserAndUid? userAndUid)
    {
        if (user != null && user.SelectedUserGameRole != null)
        {
            userAndUid = new UserAndUid(user.Entity, user.SelectedUserGameRole!);
            return true;
        }

        userAndUid = null;
        return false;
    }
}