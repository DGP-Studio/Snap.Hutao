// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Calendar;

internal sealed partial class CalendarDay : IPropertyValuesProvider
{
    public required DateTimeOffset Date { get; init; }

    public required int DayInMonth { get; init; }

    public required string DayName { get; init; }

    public required ImmutableArray<Item> BirthDayAvatars { get; init; }

    public required ImmutableArray<CalendarMaterial> Materials { get; init; }
}