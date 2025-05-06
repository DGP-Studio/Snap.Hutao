// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Post;

internal sealed class SimpleAvatar
{
    public SimpleAvatar(DetailedCharacter character)
    {
        AvatarId = character.Base.Id;
        WeaponId = character.Weapon.Id;
        ReliquarySetIds = character.Relics.Select(r => r.ReliquarySet.Id);
        ActivedConstellationNumber = character.Base.ActivedConstellationNum;
    }

    /// <summary>
    /// 角色 Id
    /// </summary>
    public AvatarId AvatarId { get; set; }

    /// <summary>
    /// 武器 Id
    /// </summary>
    public WeaponId WeaponId { get; set; }

    /// <summary>
    /// 圣遗物套装Id
    /// </summary>
    public IEnumerable<ReliquarySetId> ReliquarySetIds { get; set; }

    /// <summary>
    /// 命座
    /// </summary>
    public int ActivedConstellationNumber { get; set; }
}