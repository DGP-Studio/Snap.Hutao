// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Calendar;

internal readonly struct MonthAndDay
{
    public readonly uint Month;
    public readonly uint Day;

    public MonthAndDay(uint month, uint day)
    {
        Month = month;
        Day = day;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Month, Day);
    }
}