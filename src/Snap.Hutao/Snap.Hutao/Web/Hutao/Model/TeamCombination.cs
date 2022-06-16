// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 队伍上场率
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

/// <summary>
/// 队伍上场率2
/// </summary>
public record TeamCombination2
{
    /// <summary>
    /// 带有层的间
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// 队伍
    /// </summary>
    public IEnumerable<Rate<Team>> Teams { get; set; } = null!;
}