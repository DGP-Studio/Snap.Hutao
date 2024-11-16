// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

internal static class ServerRegionTimeZone
{
    /// <summary>
    /// UTC-05
    /// </summary>
    public static TimeSpan AmericaServerOffset { get; } = new(-05, 0, 0);

    /// <summary>
    /// UTC+01
    /// </summary>
    public static TimeSpan EuropeServerOffset { get; } = new(+01, 0, 0);

    /// <summary>
    /// UTC+08
    /// </summary>
    public static TimeSpan CommonOffset { get; } = new(+08, 0, 0);
}