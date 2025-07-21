// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed class StatisticsCultivateItem
{
    private readonly TimeSpan offset;

    private StatisticsCultivateItem(Material inner, TimeSpan offset)
    {
        Inner = inner;
        this.offset = offset;
        ExcludedFromPresentation = true;
    }

    private StatisticsCultivateItem(Material inner, Model.Entity.CultivateItem entity, TimeSpan offset)
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

    public bool IsToday { get => Inner.IsItemOfToday(offset, true); }

    internal bool ExcludedFromPresentation { get; set; }

    public static StatisticsCultivateItem Create(Material inner, TimeSpan offset)
    {
        return new(inner, offset);
    }

    public static StatisticsCultivateItem Create(Material inner, Model.Entity.CultivateItem entity, TimeSpan offset)
    {
        return new(inner, entity, offset);
    }
}