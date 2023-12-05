// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core;

internal static class UnsafeDateTimeOffset
{
    [Pure]
    [SuppressMessage("", "SH002")]
    public static unsafe DateTimeOffset AdjustOffsetOnly(DateTimeOffset dateTimeOffset, in TimeSpan offset)
    {
        return new(GetPrivateDateTime(ref dateTimeOffset), offset);
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_dateTime")]
    private static extern ref readonly DateTime GetPrivateDateTime(ref DateTimeOffset dateTimeOffset);
}