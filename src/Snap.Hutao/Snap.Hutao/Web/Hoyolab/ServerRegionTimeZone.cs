// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

internal static class ServerRegionTimeZone
{
    private static readonly TimeSpan AmericaOffsetValue = new(-05, 0, 0);
    private static readonly TimeSpan EuropeOffsetValue = new(+01, 0, 0);
    private static readonly TimeSpan CommonOffsetValue = new(+08, 0, 0);

    /// <summary>
    /// UTC-05
    /// </summary>
    public static TimeSpan AmericaServerOffset { get => AmericaOffsetValue; }

    /// <summary>
    /// UTC+01
    /// </summary>
    public static TimeSpan EuropeServerOffset { get => EuropeOffsetValue; }

    /// <summary>
    /// UTC+08
    /// </summary>
    public static TimeSpan CommonOffset { get => CommonOffsetValue; }
}