// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Monster;

/// <summary>
/// 敌对生物
/// </summary>
internal sealed class Monster
{
    internal const uint MaxLevel = 100;

    /// <summary>
    /// Id
    /// </summary>
    public MonsterId Id { get; set; }

    /// <summary>
    /// 描述 Id
    /// </summary>
    public MonsterDescribeId DescribeId { get; set; }

    /// <summary>
    /// 关系 Id
    /// </summary>
    public MonsterRelationshipId RelationshipId { get; set; }

    /// <summary>
    /// 内部代号
    /// </summary>
    public string MonsterName { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 怪物种类
    /// </summary>
    public MonsterType Type { get; set; }

    /// <summary>
    /// 始基力
    /// </summary>
    public Arkhe Arkhe { get; set; }

    /// <summary>
    /// 强化标签
    /// </summary>
    public List<string>? Affixes { get; set; } = default!;

    /// <summary>
    /// 掉落物 Id
    /// </summary>
    public List<MaterialId>? Drops { get; set; } = default!;

    /// <summary>
    /// 基本属性
    /// </summary>
    public MonsterBaseValue BaseValue { get; set; } = default!;

    /// <summary>
    /// 生长曲线
    /// </summary>
    public List<TypeValue<FightProperty, GrowCurveType>> GrowCurves { get; set; } = default!;

    /// <summary>
    /// 养成物品视图
    /// </summary>
    [JsonIgnore]
    public List<DisplayItem>? DropsView { get; set; }
}