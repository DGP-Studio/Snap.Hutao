// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

internal static class ServerTimeZoneInfo
{
    private static readonly TimeZoneInfo UsaTimeZoneValue = TimeZoneInfo.CreateCustomTimeZone("Server:UTC-05", new TimeSpan(-05, 0, 0), "UTC-05", "UTC-05");
    private static readonly TimeZoneInfo EuroTimeZoneValue = TimeZoneInfo.CreateCustomTimeZone("Server:UTC+02", new TimeSpan(+02, 0, 0), "UTC+02", "UTC+02");
    private static readonly TimeZoneInfo CommonTimeZoneValue = TimeZoneInfo.CreateCustomTimeZone("Server:UTC+08", new TimeSpan(+08, 0, 0), "UTC+08", "UTC+08");

    public static TimeZoneInfo UsaTimeZone { get => UsaTimeZoneValue; }

    public static TimeZoneInfo EuroTimeZone { get => EuroTimeZoneValue; }

    public static TimeZoneInfo CommonTimeZone { get => CommonTimeZoneValue; }
}