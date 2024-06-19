// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 背包物品
/// </summary>
[HighQuality]
[Table("inventory_items")]
internal sealed class InventoryItem : IDbMappingForeignKeyFrom<InventoryItem, uint>,
    IDbMappingForeignKeyFrom<InventoryItem, uint, uint>
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
    public uint ItemId { get; set; }

    /// <summary>
    /// 个数 4294967295
    /// </summary>
    public uint Count { get; set; }

    /// <summary>
    /// 构造一个新的个数为0的物品
    /// </summary>
    /// <param name="projectId">项目Id</param>
    /// <param name="itemId">物品Id</param>
    /// <returns>新的个数为0的物品</returns>
    public static InventoryItem From(in Guid projectId, in uint itemId)
    {
        return new()
        {
            ProjectId = projectId,
            ItemId = itemId,
        };
    }

    /// <summary>
    /// 构造一个新的个数不为0的物品
    /// </summary>
    /// <param name="projectId">项目Id</param>
    /// <param name="itemId">物品Id</param>
    /// <param name="count">物品个数</param>
    /// <returns>新的个数不为0的物品</returns>
    public static InventoryItem From(in Guid projectId, in uint itemId, in uint count)
    {
        return new()
        {
            ProjectId = projectId,
            ItemId = itemId,
            Count = count,
        };
    }
}