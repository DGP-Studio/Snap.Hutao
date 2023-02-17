// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 背包圣遗物
/// </summary>
[HighQuality]
[Table("inventory_reliquaries")]
internal sealed class InventoryReliquary
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 培养计划Id
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// 所属的计划
    /// </summary>
    [ForeignKey(nameof(ProjectId))]
    public CultivateProject Project { get; set; } = default!;

    /// <summary>
    /// 物品Id
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 主属性
    /// </summary>
    public int MainPropId { get; set; }

    /// <summary>
    /// 副词条Id
    /// </summary>
    public List<int> AppendPropIdList { get; set; } = default!;
}