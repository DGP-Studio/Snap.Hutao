// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Snap.Hutao.Core.Setting;
using System.IO;
using System.Security.Principal;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Snap.Hutao.Core;

[Injection(InjectAs.Singleton)]
internal sealed class RuntimeOptions
{
    private readonly bool isWebView2Supported;
    private readonly string webView2Version = SH.CoreWebView2HelperVersionUndetected;

    private bool? isElevated;

    public RuntimeOptions(ILogger<RuntimeOptions> logger)
    {
        AppLaunchTime = DateTimeOffset.UtcNow;

        DataFolder = GetDataFolderPath();
        LocalCache = ApplicationData.Current.LocalCacheFolder.Path;
        InstalledLocation = Package.Current.InstalledLocation.Path;
        FamilyName = Package.Current.Id.FamilyName;

        Version = Package.Current.Id.Version.ToVersion();
        UserAgent = $"Snap Hutao/{Version}";

        DeviceId = GetUniqueUserId();
        DetectWebView2Environment(logger, out webView2Version, out isWebView2Supported);

        static string GetDataFolderPath()
        {
            string preferredPath = LocalSetting.Get(SettingKeys.DataFolderPath, string.Empty);

            if (!string.IsNullOrEmpty(preferredPath))
            {
                Directory.CreateDirectory(preferredPath);
                return preferredPath;
            }

            // Fallback to MyDocuments
            string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

#if RELEASE
            // 将测试版与正式版的文件目录分离
            string folderName = Package.Current.PublisherDisplayName == "DGP Studio CI" ? "HutaoAlpha" : "Hutao";
#else
            // 使得迁移能正常生成
            string folderName = "Hutao";
#endif
            string path = Path.GetFullPath(Path.Combine(myDocuments, folderName));
            Directory.CreateDirectory(path);
            return path;
        }

        static string GetUniqueUserId()
        {
            string userName = Environment.UserName;
            object? machineGuid = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography\", "MachineGuid", userName);
            return Convert.ToMd5HexString($"{userName}{machineGuid}");
        }

        static void DetectWebView2Environment(ILogger<RuntimeOptions> logger, out string webView2Version, out bool isWebView2Supported)
        {
            try
            {
                webView2Version = CoreWebView2Environment.GetAvailableBrowserVersionString();
                isWebView2Supported = true;
            }
            catch (FileNotFoundException ex)
            {
                webView2Version = SH.CoreWebView2HelperVersionUndetected;
                isWebView2Supported = false;
                logger.LogError(ex, "WebView2 Runtime not installed.");
            }
        }
    }

    public Version Version { get; }

    public string UserAgent { get; }

    public string InstalledLocation { get; }

    public string DataFolder { get; }

    public string LocalCache { get; }

    public string FamilyName { get; }

    public string DeviceId { get; }

    public string WebView2Version { get => webView2Version; }

    public bool IsWebView2Supported { get => isWebView2Supported; }

    public bool IsElevated
    {
        get
        {
            return isElevated ??= GetElevated();

            static bool GetElevated()
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
    }

    public DateTimeOffset AppLaunchTime { get; }
}