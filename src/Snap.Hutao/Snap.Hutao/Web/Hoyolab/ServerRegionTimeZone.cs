// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

internal static class ServerRegionTimeZone
{
    /// <summary>
    /// UTC-05
    /// </summary>
    public static TimeSpan AmericaServerOffset { get; } = TimeSpan.FromHours(-05);

    /// <summary>
    /// UTC+01
    /// </summary>
    public static TimeSpan EuropeServerOffset { get; } = TimeSpan.FromHours(+01);

    /// <summary>
    /// UTC+08
    /// </summary>
    public static TimeSpan CommonOffset { get; } = TimeSpan.FromHours(+08);
}