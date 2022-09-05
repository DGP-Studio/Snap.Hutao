// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 玩家深渊战斗间信息
/// </summary>
public class PlayerSpiralAbyssLevel
{
    /// <summary>
    /// 构造一个新的玩家深渊战斗间信息
    /// </summary>
    /// <param name="indexedLevel">楼层</param>
    internal PlayerSpiralAbyssLevel(IndexedLevel indexedLevel)
    {
        FloorIndex = indexedLevel.FloorIndex;
        LevelIndex = indexedLevel.Level.Index;
        Star = indexedLevel.Level.Star;
        Battles = indexedLevel.Level.Battles
            .Select(battle => new PlayerSpiralAbyssBattle(battle));
    }

    /// <summary>
    /// 层号
    /// </summary>
    public int FloorIndex { get; }

    /// <summary>
    /// 间号
    /// </summary>
    public int LevelIndex { get; }

    /// <summary>
    /// 星数
    /// </summary>
    public int Star { get; }

    /// <summary>
    /// 战斗列表 分上下半间
    /// </summary>
    public IEnumerable<PlayerSpiralAbyssBattle> Battles { get; }
}
