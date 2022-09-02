// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using System.Linq;

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 玩家角色
/// </summary>
public class PlayerAvatar
{
    /// <summary>
    /// 构造一个新的玩家角色
    /// </summary>
    /// <param name="avatar">角色</param>
    internal PlayerAvatar(Character avatar)
    {
        Id = avatar.Id;
        Level = avatar.Level;
        ActivedConstellationNum = avatar.ActivedConstellationNum;
        Weapon = new(avatar.Weapon.Id, avatar.Weapon.Level, avatar.Weapon.AffixLevel);
        ReliquarySets = avatar.Reliquaries
            .CountBy(relic => relic.ReliquarySet.Id)
            .Select(kvp => new AvatarReliquarySet(kvp))
            .ToList();
    }

    /// <summary>
    /// 角色Id
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 角色等级
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// 命座
    /// </summary>
    public int ActivedConstellationNum { get; }

    /// <summary>
    /// 武器
    /// </summary>
    public AvatarWeapon Weapon { get; }

    /// <summary>
    /// 圣遗物套装
    /// </summary>
    public List<AvatarReliquarySet> ReliquarySets { get; }
}
