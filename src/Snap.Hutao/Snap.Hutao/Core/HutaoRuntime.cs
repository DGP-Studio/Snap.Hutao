// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Microsoft.Windows.AppNotifications;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Setting;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Snap.Hutao.Core;

internal static class HutaoRuntime
{
    public static Version Version { get; } = Package.Current.Id.Version.ToVersion();

    public static string UserAgent { get; } = $"Snap Hutao/{Version}";

    public static string DataDirectory { get; } = InitializeDataDirectory();

    public static string LocalCacheDirectory { get; } = ApplicationData.Current.LocalCacheFolder.Path;

    public static string FamilyName { get; } = Package.Current.Id.FamilyName;

    public static string DeviceId { get; } = InitializeDeviceId();

    public static WebView2Version WebView2Version { get; } = InitializeWebView2();

    public static bool IsProcessElevated { get; } = LocalSetting.Get(SettingKeys.OverrideElevationRequirement, false) || Environment.IsPrivilegedProcess;

    // Requires main thread
    public static bool IsAppNotificationEnabled { get; } = AppNotificationManager.Default.Setting is AppNotificationSetting.Enabled;

    public static string? GetDisplayName()
    {
        // AppNameAndVersion
        // AppDevNameAndVersion
        // AppElevatedNameAndVersion
        // AppElevatedDevNameAndVersion
        string name = new StringBuilder()
            .Append("App")
            .AppendIf(IsProcessElevated, "Elevated")
#if DEBUG
            .Append("Dev")
#endif
            .Append("NameAndVersion")
            .ToString();

        Debug.Assert(XamlApplicationLifetime.CultureInfoInitialized);
        string? displayName = SH.GetString(name, Version);
        return displayName is null ? null : string.Intern(displayName);
    }

    public static ValueFile GetDataDirectoryFile(string fileName)
    {
        return string.Intern(Path.Combine(DataDirectory, fileName));
    }

    public static ValueFile GetDataUpdateCacheDirectoryFile(string fileName)
    {
        string directory = Path.Combine(DataDirectory, "UpdateCache");
        Directory.CreateDirectory(directory);
        return string.Intern(Path.Combine(directory, fileName));
    }

    public static ValueDirectory GetDataServerCacheDirectory()
    {
        string directory = Path.Combine(DataDirectory, "ServerCache");
        Directory.CreateDirectory(directory);
        return string.Intern(directory);
    }

    public static ValueDirectory GetDataBackgroundDirectory()
    {
        string directory = Path.Combine(DataDirectory, "Background");
        Directory.CreateDirectory(directory);
        return string.Intern(directory);
    }

    public static ValueDirectory GetDataScreenshotDirectory()
    {
        string directory = Path.Combine(DataDirectory, "Screenshot");
        Directory.CreateDirectory(directory);
        return string.Intern(directory);
    }

    public static ValueDirectory GetLocalCacheImageCacheDirectory()
    {
        string directory = Path.Combine(LocalCacheDirectory, "ImageCache");
        Directory.CreateDirectory(directory);
        return string.Intern(directory);
    }

    private static string InitializeDataDirectory()
    {
        // Delete the previous data folder if it exists
        try
        {
            string previousDirectory = LocalSetting.Get(SettingKeys.PreviousDataDirectoryToDelete, string.Empty);
            if (!string.IsNullOrEmpty(previousDirectory) && Directory.Exists(previousDirectory))
            {
                Directory.Delete(previousDirectory, true);
            }
        }
        finally
        {
            LocalSetting.Set(SettingKeys.PreviousDataDirectoryToDelete, string.Empty);
        }

        // Check if the preferred path is set
        string currentDirectory = LocalSetting.Get(SettingKeys.DataDirectory, string.Empty);

        if (!string.IsNullOrEmpty(currentDirectory))
        {
            Directory.CreateDirectory(currentDirectory);
            return currentDirectory;
        }

        const string FolderName
#if IS_ALPHA_BUILD
        = "HutaoAlpha";
#elif IS_CANARY_BUILD
        = "HutaoCanary";
#else
        = "Hutao";
#endif

        // Check if the old documents path exists
        string myDocumentsHutaoDirectory = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), FolderName));
        if (Directory.Exists(myDocumentsHutaoDirectory))
        {
            LocalSetting.Set(SettingKeys.DataDirectory, myDocumentsHutaoDirectory);
            return myDocumentsHutaoDirectory;
        }

        // Prefer LocalApplicationData
        string localApplicationData = ApplicationData.Current.LocalFolder.Path;
        string path = Path.GetFullPath(Path.Combine(localApplicationData, FolderName));
        try
        {
            Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
            // FileNotFoundException | UnauthorizedAccessException
            // We don't have enough permission
            HutaoException.InvalidOperation($"Failed to create data folder: {path}", ex);
        }

        LocalSetting.Set(SettingKeys.DataDirectory, path);
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
            return new(version, version, true);
        }
        catch (FileNotFoundException)
        {
            return new(string.Empty, SH.CoreWebView2HelperVersionUndetected, false);
        }
    }
}