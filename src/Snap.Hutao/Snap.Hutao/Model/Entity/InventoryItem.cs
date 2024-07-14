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
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public Guid ProjectId { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public CultivateProject Project { get; set; } = default!;

    public uint ItemId { get; set; }

    public uint Count { get; set; }

    public static InventoryItem From(Guid projectId, uint itemId)
    {
        return new()
        {
            ProjectId = projectId,
            ItemId = itemId,
        };
    }

    public static InventoryItem From(Guid projectId, uint itemId, uint count)
    {
        return new()
        {
            ProjectId = projectId,
            ItemId = itemId,
            Count = count,
        };
    }
}