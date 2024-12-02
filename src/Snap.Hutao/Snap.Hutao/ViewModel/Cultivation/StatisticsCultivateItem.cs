// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed class StatisticsCultivateItem
{
    public StatisticsCultivateItem(Material inner, Model.Entity.CultivateItem entity)
    {
        Inner = inner;
        Count = (int)entity.Count;
    }

    public Material Inner { get; }

    public int Count { get; set; }

    public int Current { get; set; }

    public bool IsFinished { get => Current >= Count; }

    public string CountFormatted { get => $"{Current}/{Count}"; }

    public bool IsToday { get => Inner.IsTodaysItem(true); }
}