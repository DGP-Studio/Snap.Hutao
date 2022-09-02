// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 队伍上场率
/// 层间上场率
/// </summary>
public record TeamCombination
{
    /// <summary>
    /// 带有层的间
    /// </summary>
    public LevelInfo Level { get; set; } = null!;

    /// <summary>
    /// 队伍
    /// </summary>
    public IEnumerable<Rate<Team>> Teams { get; set; } = null!;
}