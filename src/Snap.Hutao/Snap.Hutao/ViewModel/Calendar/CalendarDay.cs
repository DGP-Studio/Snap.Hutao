// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Calendar;

internal sealed partial class CalendarDay : IAdvancedCollectionViewItem
{
    public DateTimeOffset Date { get; set; }

    public int DayInMonth { get; set; }

    public string DayName { get; set; } = default!;

    public ImmutableArray<Avatar> BirthDayAvatars { get; set; }
}