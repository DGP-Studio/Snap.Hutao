// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Extension;

internal struct UnsafeDateTimeOffset
{
    private DateTime dateTime;
    private short offsetMinutes;

    public DateTime DateTime { readonly get => dateTime; set => dateTime = value; }

    [SuppressMessage("", "SH002")]
    public static unsafe DateTimeOffset AdjustOffsetOnly(DateTimeOffset dateTimeOffset, in TimeSpan offset)
    {
        UnsafeDateTimeOffset* pUnsafe = (UnsafeDateTimeOffset*)&dateTimeOffset;
        pUnsafe->offsetMinutes = (short)(offset.Ticks / TimeSpan.TicksPerMinute);
        return dateTimeOffset;
    }
}