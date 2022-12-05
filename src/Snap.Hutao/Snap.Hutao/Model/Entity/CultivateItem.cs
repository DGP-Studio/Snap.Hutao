// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 消耗物品
/// </summary>
[Table("cultivate_items")]
public class CultivateItem
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 外键
    /// </summary>
    public Guid EntryId { get; set; }

    /// <summary>
    /// 入口名称
    /// </summary>
    [ForeignKey(nameof(EntryId))]
    public CultivateEntry Entry { get; set; } = default!;

    /// <summary>
    /// 物品 Id
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// 物品个数
    /// </summary>
    public int Count { get; set; }
}