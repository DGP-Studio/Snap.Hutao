// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

/// <summary>
/// 队伍出场次数
/// </summary>
[HighQuality]
internal sealed class TeamAppearance
{
    /// <summary>
    /// 层
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// 上半
    /// </summary>
    public List<ItemRate<string, int>> Up { get; set; } = default!;

    /// <summary>
    /// 下半
    /// </summary>
    public List<ItemRate<string, int>> Down { get; set; } = default!;
}