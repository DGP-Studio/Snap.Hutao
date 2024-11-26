// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Microsoft.Windows.AppNotifications;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Setting;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Snap.Hutao.Core;

internal static class HutaoRuntime
{
    public static Version Version { get; } = Package.Current.Id.Version.ToVersion();

    public static string UserAgent { get; } = $"Snap Hutao/{Version}";

    public static string DataFolder { get; } = InitializeDataFolder();

    public static string LocalCache { get; } = ApplicationData.Current.LocalCacheFolder.Path;

    public static string FamilyName { get; } = Package.Current.Id.FamilyName;

    public static string FullName { get; } = Package.Current.Id.FullName;

    public static string DeviceId { get; } = InitializeDeviceId();

    public static WebView2Version WebView2Version { get; } = InitializeWebView2();

    public static bool IsProcessElevated { get; } = InitializeIsElevated();

    // Requires main thread
    public static bool IsAppNotificationEnabled { get; } = AppNotificationManager.Default.Setting is AppNotificationSetting.Enabled;

    public static DateTimeOffset LaunchTime { get; } = DateTimeOffset.UtcNow;

    public static string GetDataFolderFile(string fileName)
    {
        return Path.Combine(DataFolder, fileName);
    }

    public static string GetDataFolderUpdateCacheFolderFile(string fileName)
    {
        string directory = Path.Combine(DataFolder, "UpdateCache");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, fileName);
    }

    public static string GetDataFolderServerCacheFolder()
    {
        string directory = Path.Combine(DataFolder, "ServerCache");
        Directory.CreateDirectory(directory);
        return directory;
    }

    public static string GetDataFolderBackgroundFolder()
    {
        string directory = Path.Combine(DataFolder, "Background");
        Directory.CreateDirectory(directory);
        return directory;
    }

    public static string GetLocalCacheImageCacheFolder()
    {
        string directory = Path.Combine(LocalCache, "ImageCache");
        Directory.CreateDirectory(directory);
        return directory;
    }

    private static string InitializeDataFolder()
    {
        string preferredPath = LocalSetting.Get(SettingKeys.DataFolderPath, string.Empty);

        if (!string.IsNullOrEmpty(preferredPath))
        {
            Directory.CreateDirectory(preferredPath);
            return preferredPath;
        }

        // Fallback to MyDocuments
        string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

#if IS_ALPHA_BUILD
        const string FolderName = "HutaoAlpha";
#else
        // 使得迁移能正常生成
        const string FolderName = "Hutao";
#endif
        string path = Path.GetFullPath(Path.Combine(myDocuments, FolderName));
        Directory.CreateDirectory(path);
        return path;
    }

    private static string InitializeDeviceId()
    {
        string userName = Environment.UserName;
        object? machineGuid = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography\", "MachineGuid", userName);
        return Hash.ToHexString(HashAlgorithmName.MD5, $"{userName}{machineGuid}");
    }

    private static WebView2Version InitializeWebView2()
    {
        try
        {
            string version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            return new(version, true);
        }
        catch (FileNotFoundException)
        {
            return new(SH.CoreWebView2HelperVersionUndetected, false);
        }
    }

    private static bool InitializeIsElevated()
    {
        if (LocalSetting.Get(SettingKeys.OverrideElevationRequirement, false))
        {
            return true;
        }

        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}