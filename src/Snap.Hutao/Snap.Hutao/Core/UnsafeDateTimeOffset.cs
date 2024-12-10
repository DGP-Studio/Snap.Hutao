// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core;

internal static class UnsafeDateTimeOffset
{
    public static DateTimeOffset ParseDateTime(ReadOnlySpan<char> span, TimeSpan offset)
    {
        return new(DateTime.Parse(span, CultureInfo.InvariantCulture), offset);
    }

    [Pure]
    public static DateTimeOffset AdjustOffsetOnly(DateTimeOffset dateTimeOffset, in TimeSpan offset)
    {
        return new(GetPrivateDateTime(in dateTimeOffset), offset);
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_dateTime")]
    private static extern ref DateTime GetPrivateDateTime(ref readonly DateTimeOffset dateTimeOffset);
}