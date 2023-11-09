// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

internal static class ServerTimeZoneInfo
{
    private static readonly TimeZoneInfo AmericaTimeZoneValue = TimeZoneInfo.CreateCustomTimeZone("Server:UTC-05", new TimeSpan(-05, 0, 0), "UTC-05", "UTC-05");
    private static readonly TimeZoneInfo EuropeTimeZoneValue = TimeZoneInfo.CreateCustomTimeZone("Server:UTC+01", new TimeSpan(+01, 0, 0), "UTC+01", "UTC+01");
    private static readonly TimeZoneInfo CommonTimeZoneValue = TimeZoneInfo.CreateCustomTimeZone("Server:UTC+08", new TimeSpan(+08, 0, 0), "UTC+08", "UTC+08");

    public static TimeZoneInfo AmericaTimeZone { get => AmericaTimeZoneValue; }

    public static TimeZoneInfo EuropeTimeZone { get => EuropeTimeZoneValue; }

    public static TimeZoneInfo CommonTimeZone { get => CommonTimeZoneValue; }
}