// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Tower;

/// <summary>
/// 深渊 间
/// </summary>
internal sealed class TowerLevel
{
    /// <summary>
    /// Id
    /// </summary>
    public TowerLevelId Id { get; set; }

    /// <summary>
    /// 深渊间分组编号
    /// </summary>
    public TowerLevelGroupId GroupId { get; set; }

    /// <summary>
    /// 编号
    /// </summary>
    public uint Index { get; set; }

    /// <summary>
    /// 怪物等级
    /// </summary>
    public uint MonsterLevel { get; set; }

    /// <summary>
    /// 上半怪物预览
    /// </summary>
    public List<MonsterId> FirstMonsters { get; set; } = default!;

    /// <summary>
    /// 下半怪物预览
    /// </summary>
    public List<MonsterId> SecondMonsters { get; set; } = default!;
}