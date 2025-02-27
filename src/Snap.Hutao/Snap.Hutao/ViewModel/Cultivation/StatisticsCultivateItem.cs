// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed class StatisticsCultivateItem
{
    private readonly TimeSpan offset;

    private StatisticsCultivateItem(Material inner, Model.Entity.CultivateItem entity, in TimeSpan offset)
    {
        Inner = inner;
        Count = entity.Count;
        this.offset = offset;
    }

    public Material Inner { get; }

    public uint Count { get; set; }

    public uint Current { get; set; }

    public bool IsFinished { get => Current >= Count; }

    public string FormattedCount { get => $"{Current}/{Count}"; }

    public bool IsToday { get => Inner.IsTodaysItem(offset, true); }

    public static StatisticsCultivateItem Create(Material inner, Model.Entity.CultivateItem entity, in TimeSpan offset)
    {
        return new(inner, entity, offset);
    }
}