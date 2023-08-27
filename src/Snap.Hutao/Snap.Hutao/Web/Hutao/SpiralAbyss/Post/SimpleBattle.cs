// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Post;

/// <summary>
/// 上下半信息
/// </summary>
[HighQuality]
internal sealed class SimpleBattle
{
    /// <summary>
    /// 构造一个新的战斗
    /// </summary>
    /// <param name="battle">战斗</param>
    public SimpleBattle(Battle battle)
    {
        Index = battle.Index;
        Avatars = battle.Avatars.Select(a => a.Id);
    }

    /// <summary>
    /// 上下半遍号 1-2
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 角色列表
    /// </summary>
    public IEnumerable<AvatarId> Avatars { get; set; } = default!;
}
