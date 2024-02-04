// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
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
    private readonly IServiceProvider serviceProvider;

    private readonly Lazy<(Version Version, string UserAgent)> lazyVersionAndUserAgent = new(() =>
    {
        Version version = Package.Current.Id.Version.ToVersion();
        return (version, $"Snap Hutao/{version}");
    });

    private readonly Lazy<string> lazyDataFolder = new(() =>
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
    });

    private readonly Lazy<string> lazyDeviceId = new(() =>
    {
        string userName = Environment.UserName;
        object? machineGuid = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography\", "MachineGuid", userName);
        return Convert.ToMd5HexString($"{userName}{machineGuid}");
    });

    private readonly Lazy<(string Version, bool Supported)> lazyWebViewEnvironment = new(() =>
    {
        try
        {
            string version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            return (version, true);
        }
        catch (FileNotFoundException)
        {
            return (SH.CoreWebView2HelperVersionUndetected, false);
        }
    });

    private readonly Lazy<bool> lazyElevated = new(() =>
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
    });

    private readonly Lazy<string> lazyLocalCache = new(() => ApplicationData.Current.LocalCacheFolder.Path);
    private readonly Lazy<string> lazyInstalledLocation = new(() => Package.Current.InstalledLocation.Path);
    private readonly Lazy<string> lazyFamilyName = new(() => Package.Current.Id.FamilyName);

    private bool isToastAvailable;
    private bool isToastAvailableInitialized;
    private object locker = new();

    public RuntimeOptions(IServiceProvider serviceProvider, ILogger<RuntimeOptions> logger)
    {
        this.serviceProvider = serviceProvider;

        AppLaunchTime = DateTimeOffset.UtcNow;
    }

    public Version Version { get => lazyVersionAndUserAgent.Value.Version; }

    public string UserAgent { get => lazyVersionAndUserAgent.Value.UserAgent; }

    public string InstalledLocation { get => lazyInstalledLocation.Value; }

    public string DataFolder { get => lazyDataFolder.Value; }

    public string LocalCache { get => lazyLocalCache.Value; }

    public string FamilyName { get => lazyFamilyName.Value; }

    public string DeviceId { get => lazyDeviceId.Value; }

    public string WebView2Version { get => lazyWebViewEnvironment.Value.Version; }

    public bool IsWebView2Supported { get => lazyWebViewEnvironment.Value.Supported; }

    public bool IsElevated { get => lazyElevated.Value; }

    public bool IsToastAvailable
    {
        get
        {
            return LazyInitializer.EnsureInitialized(ref isToastAvailable, ref isToastAvailableInitialized, ref locker, () =>
            {
                return serviceProvider.GetRequiredService<ITaskContext>().InvokeOnMainThread(() =>
                {
                    return ToastNotificationManagerCompat.CreateToastNotifier().Setting is Windows.UI.Notifications.NotificationSetting.Enabled;
                });
            });
        }
    }

    public DateTimeOffset AppLaunchTime { get; }
}