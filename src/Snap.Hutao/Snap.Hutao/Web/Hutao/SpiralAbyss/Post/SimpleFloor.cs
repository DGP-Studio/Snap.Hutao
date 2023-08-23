// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Post;

/// <summary>
/// 层信息
/// </summary>
[HighQuality]
internal sealed class SimpleFloor
{
    /// <summary>
    /// 构造一个新的层信息
    /// </summary>
    /// <param name="floor">层信息</param>
    public SimpleFloor(Floor floor)
    {
        Index = floor.Index;
        Star = floor.Star;
        Levels = floor.Levels.Select(l => new SimpleLevel(l));
    }

    /// <summary>
    /// 层遍号 1-12|9-12
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 星数
    /// </summary>
    public int Star { get; set; }

    /// <summary>
    /// 间
    /// </summary>
    public IEnumerable<SimpleLevel> Levels { get; set; } = default!;
}
