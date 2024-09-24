// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Calendar;

internal sealed class CalendarDay
{
    public DateTimeOffset Date { get; set; }

    public int DayInMonth { get; set; }

    public bool IsToday { get; set; }
}