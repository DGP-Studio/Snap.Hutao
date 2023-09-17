// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Tower;

internal sealed class TowerWave
{
    /// <summary>
    /// 波次类型
    /// </summary>
    public WaveType Type { get; set; }

    /// <summary>
    /// 额外描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 分波怪物
    /// </summary>
    public List<TowerMonster> Monsters { get; set; } = default!;
}