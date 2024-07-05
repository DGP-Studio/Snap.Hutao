// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="DateTimeOffset"/> 扩展
/// </summary>
[HighQuality]
internal static class DateTimeOffsetExtension
{
    public static readonly DateTimeOffset DatebaseDefaultTime = new(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0));

    public static DateTimeOffset UnsafeRelaxedFromUnixTime(long? timestamp, in DateTimeOffset defaultValue)
    {
        if (timestamp is not { } value)
        {
            return defaultValue;
        }

        return value switch
        {
            >= -62135596800L and <= 253402300799L => DateTimeOffset.FromUnixTimeSeconds(value),
            >= -62135596800000L and <= 253402300799999L => DateTimeOffset.FromUnixTimeMilliseconds(value),
            _ => defaultValue,
        };
    }
}