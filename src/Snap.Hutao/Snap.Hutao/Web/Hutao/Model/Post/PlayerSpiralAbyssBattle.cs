// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 玩家深渊某间的战斗信息
/// </summary>
public class PlayerSpiralAbyssBattle
{
    /// <summary>
    /// 构造一个新的战斗信息
    /// </summary>
    /// <param name="battle">战斗</param>
    internal PlayerSpiralAbyssBattle(Battle battle)
    {
        BattleIndex = battle.Index;
        AvatarIds = battle.Avatars.Select(a => a.Id);
    }

    /// <summary>
    /// 战斗上下半间 0,1
    /// </summary>
    public int BattleIndex { get; }

    /// <summary>
    /// 角色Id列表
    /// </summary>
    public IEnumerable<int> AvatarIds { get; }
}