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

    /// <summary>
    /// 从Unix时间戳转换
    /// </summary>
    /// <param name="timestamp">时间戳</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>转换的时间</returns>
    public static DateTimeOffset FromUnixTime(long? timestamp, in DateTimeOffset defaultValue)
    {
        if (timestamp is { } value)
        {
            try
            {
                return DateTimeOffset.FromUnixTimeSeconds(value);
            }
            catch (ArgumentOutOfRangeException)
            {
                try
                {
                    return DateTimeOffset.FromUnixTimeMilliseconds(value);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return defaultValue;
                }
            }
        }
        else
        {
            return defaultValue;
        }
    }
}