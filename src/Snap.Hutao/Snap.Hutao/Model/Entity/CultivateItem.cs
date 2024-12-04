// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("cultivate_items")]
internal sealed class CultivateItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public Guid EntryId { get; set; }

    [ForeignKey(nameof(EntryId))]
    public CultivateEntry Entry { get; set; } = default!;

    public uint ItemId { get; set; }

    public uint Count { get; set; }

    public bool IsFinished { get; set; }

    public static CultivateItem From(Guid entryId, Web.Hoyolab.Takumi.Event.Calculate.Item item)
    {
        return new()
        {
            EntryId = entryId,
            ItemId = item.Id,
            Count = item.Num,
        };
    }
}