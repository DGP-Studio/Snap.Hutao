// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed class StatisticsCultivateItem
{
    private StatisticsCultivateItem(Material inner, Model.Entity.CultivateItem entity)
    {
        Inner = inner;
        Count = entity.Count;
    }

    public Material Inner { get; }

    public uint Count { get; set; }

    public uint Current { get; set; }

    public bool IsFinished { get => Current >= Count; }

    public string FormattedCount { get => $"{Current}/{Count}"; }

    public bool IsToday { get => Inner.IsTodaysItem(true); }

    public static StatisticsCultivateItem Create(Material inner, Model.Entity.CultivateItem entity)
    {
        return new(inner, entity);
    }
}