// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Model.InterChange;

internal sealed class HutaoReservedCultivationItem : IMappingFrom<HutaoReservedCultivationItem, CultivateItem>
{
    public required uint ItemId { get; set; }

    public required uint Count { get; set; }

    public required bool IsFinished { get; set; }

    public static HutaoReservedCultivationItem From(CultivateItem item)
    {
        return new()
        {
            ItemId = item.ItemId,
            Count = item.Count,
            IsFinished = item.IsFinished,
        };
    }
}