// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed class StatisticsCultivateItem
{
    public StatisticsCultivateItem(Material inner, Model.Entity.CultivateItem entity)
    {
        Inner = inner;
        Count = entity.Count;
    }

    public Material Inner { get; }

    public uint Count { get; set; }

    public uint TotalCount { get; set; }

    public bool IsFinished { get => TotalCount >= Count; }

    public string CountFormatted { get => $"{TotalCount}/{Count}"; }

    public bool IsToday { get => Inner.IsTodaysItem(true); }
}