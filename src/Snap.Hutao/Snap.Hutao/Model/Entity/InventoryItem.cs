// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("inventory_items")]
internal sealed class InventoryItem
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