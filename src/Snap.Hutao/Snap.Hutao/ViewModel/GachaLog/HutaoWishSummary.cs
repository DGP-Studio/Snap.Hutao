// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.GachaLog;

internal sealed class HutaoWishSummary
{
    public required GachaEvent Event { get; init; }

    public required ImmutableArray<StatisticsItem> UpItems { get; init; }

    public required ImmutableArray<StatisticsItem> OrangeItems { get; init; }

    public required ImmutableArray<StatisticsItem> PurpleItems { get; init; }

    public required ImmutableArray<StatisticsItem> BlueItems { get; init; }
}