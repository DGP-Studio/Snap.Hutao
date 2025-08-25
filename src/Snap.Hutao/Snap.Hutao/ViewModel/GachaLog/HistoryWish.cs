// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.GachaLog;

internal sealed partial class HistoryWish : Wish, IPropertyValuesProvider
{
    public required string Version { get; init; }

    public required Uri BannerImage { get; init; }

    public required ImmutableArray<StatisticsItem> OrangeUpList { get; init; }

    public required ImmutableArray<StatisticsItem> PurpleUpList { get; init; }

    public required ImmutableArray<StatisticsItem> OrangeList { get; init; }

    public required ImmutableArray<StatisticsItem> PurpleList { get; init; }

    public required ImmutableArray<StatisticsItem> BlueList { get; init; }
}