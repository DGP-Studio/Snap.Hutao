// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Item;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Calendar;

internal sealed class CalendarMaterial
{
    public required Material Inner { get; init; }

    public required ImmutableArray<Item> Items { get; init; }
}