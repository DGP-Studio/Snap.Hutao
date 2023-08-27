// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Post;

/// <summary>
/// 间信息
/// </summary>
[HighQuality]
internal sealed class SimpleLevel
{
    /// <summary>
    /// 构造一个新的间信息
    /// </summary>
    /// <param name="level">间信息</param>
    public SimpleLevel(Level level)
    {
        Index = level.Index;
        Star = level.Star;
        Battles = level.Battles.Select(b => new SimpleBattle(b));
    }

    /// <summary>
    /// 间遍号 1-3
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 星数
    /// </summary>
    public int Star { get; set; }

    /// <summary>
    /// 上下半信息
    /// </summary>
    public IEnumerable<SimpleBattle> Battles { get; set; } = default!;
}
