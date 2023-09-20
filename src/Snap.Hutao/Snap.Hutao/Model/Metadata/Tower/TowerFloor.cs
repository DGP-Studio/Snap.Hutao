// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Tower;

/// <summary>
/// 深渊 层
/// </summary>
internal sealed class TowerFloor
{
    /// <summary>
    /// Id
    /// </summary>
    public TowerFloorId Id { get; set; }

    /// <summary>
    /// 编号 [1-12]
    /// </summary>
    public uint Index { get; set; }

    /// <summary>
    /// 深渊间分组编号
    /// </summary>
    public TowerLevelGroupId LevelGroupId { get; set; }

    /// <summary>
    /// 背景图片
    /// </summary>
    public string Background { get; set; } = default!;

    /// <summary>
    /// 地脉紊乱
    /// </summary>
    public List<string> Descriptions { get; set; } = default!;
}