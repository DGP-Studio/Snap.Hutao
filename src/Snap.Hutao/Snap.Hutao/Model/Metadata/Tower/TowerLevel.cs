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
    public List<MonsterRelationshipId> FirstMonsters { get; set; } = default!;

    /// <summary>
    /// 上半怪物波次
    /// </summary>
    public List<TowerWave> FirstWaves { get; set; } = default!;

    /// <summary>
    /// 上半造物
    /// </summary>
    public NameDescription? FirstGadget { get; set; }

    /// <summary>
    /// 下半怪物预览
    /// </summary>
    public List<MonsterRelationshipId>? SecondMonsters { get; set; }

    /// <summary>
    /// 下半怪物波次
    /// </summary>
    public List<TowerWave> SecondWaves { get; set; } = default!;

    /// <summary>
    /// 下半造物
    /// </summary>
    public NameDescription? SecondGadget { get; set; }
}