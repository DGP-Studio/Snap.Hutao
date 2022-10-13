// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;
/// <summary>
/// 队伍出场次数
/// </summary>
public class TeamAppearance
{
    /// <summary>
    /// 上半
    /// </summary>
    public List<ItemRate<string, int>> Up { get; set; } = default!;

    /// <summary>
    /// 下半
    /// </summary>
    public List<ItemRate<string, int>> Down { get; set; } = default!;
}