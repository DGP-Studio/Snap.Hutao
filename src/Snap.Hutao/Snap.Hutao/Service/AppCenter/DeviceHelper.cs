// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.Win32;
using Windows.Graphics;

namespace Snap.Hutao.Service.AppCenter;

/// <summary>
/// 设备帮助类
/// </summary>
[SuppressMessage("", "SA1600")]
public static class DeviceHelper
{
    private static readonly RegistryKey? BiosKey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\BIOS");
    private static readonly RegistryKey? GeoKey = Registry.CurrentUser.OpenSubKey("Control Panel\\International\\Geo");
    private static readonly RegistryKey? CurrentVersionKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");

    public static string? GetOem()
    {
        string? oem = BiosKey?.GetValue("SystemManufacturer") as string;
        return oem == "System manufacturer" ? null : oem;
    }

    public static string? GetModel()
    {
        string? model = BiosKey?.GetValue("SystemProductName") as string;
        return model == "System Product Name" ? null : model;
    }

    public static string GetScreenSize()
    {
        RectInt32 screen = DisplayArea.Primary.OuterBounds;
        return $"{screen.Width}x{screen.Height}";
    }

    public static string? GetCountry()
    {
        return GeoKey?.GetValue("Name") as string;
    }

    public static string GetSystemVersion()
    {
        object? majorVersion = CurrentVersionKey?.GetValue("CurrentMajorVersionNumber");
        if (majorVersion != null)
        {
            object? minorVersion = CurrentVersionKey?.GetValue("CurrentMinorVersionNumber", "0");
            object? buildNumber = CurrentVersionKey?.GetValue("CurrentBuildNumber", "0");
            return $"{majorVersion}.{minorVersion}.{buildNumber}";
        }
        else
        {
            object? version = CurrentVersionKey?.GetValue("CurrentVersion", "0.0");
            object? buildNumber = CurrentVersionKey?.GetValue("CurrentBuild", "0");
            return $"{version}.{buildNumber}";
        }
    }

    public static int GetSystemBuild()
    {
        return (int)(CurrentVersionKey?.GetValue("UBR") ?? 0);
    }
}
