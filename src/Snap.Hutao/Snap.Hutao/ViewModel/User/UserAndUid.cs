// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Web.Hoyolab;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.ViewModel.User;

/// <summary>
/// 实体用户与角色
/// 由于许多操作需要同时用到ck与uid
/// 抽象此类用于简化这类调用
/// </summary>
[HighQuality]
internal sealed class UserAndUid : IMappingFrom<UserAndUid, EntityUser, PlayerUid>
{
    /// <summary>
    /// 构造一个新的实体用户与角色
    /// </summary>
    /// <param name="user">实体用户</param>
    /// <param name="role">角色</param>
    public UserAndUid(EntityUser user, in PlayerUid role)
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

    public bool IsOversea { get => User.IsOversea; }

    [SuppressMessage("", "SH002")]
    public static UserAndUid From(EntityUser user, PlayerUid role)
    {
        return new(user, role);
    }

    /// <summary>
    /// 尝试转换到用户与角色
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="userAndUid">用户与角色</param>
    /// <returns>是否转换成功</returns>
    public static bool TryFromUser([NotNullWhen(true)] User? user, [NotNullWhen(true)] out UserAndUid? userAndUid)
    {
        if (user is not null && user.SelectedUserGameRole is not null)
        {
            userAndUid = new UserAndUid(user.Entity, user.SelectedUserGameRole);
            return true;
        }

        userAndUid = null;
        return false;
    }
}