// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Microsoft.Windows.AppNotifications;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Setting;
using System.IO;
using System.Security.Principal;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Snap.Hutao.Core;

[Injection(InjectAs.Singleton)]
internal sealed class RuntimeOptions
{
    private readonly LazySlim<(Version Version, string UserAgent)> lazyVersionAndUserAgent = new(() =>
    {
        Version version = Package.Current.Id.Version.ToVersion();
        return (version, $"Snap Hutao/{version}");
    });

    private readonly LazySlim<string> lazyDataFolder = new(() =>
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

    private readonly LazySlim<string> lazyDeviceId = new(() =>
    {
        string userName = Environment.UserName;
        object? machineGuid = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography\", "MachineGuid", userName);
        return Hash.MD5HexString($"{userName}{machineGuid}");
    });

    private readonly LazySlim<(string Version, bool Supported)> lazyWebViewEnvironment = new(() =>
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

    private readonly LazySlim<bool> lazyElevated = new(() =>
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

    private readonly LazySlim<string> lazyLocalCache = new(() => ApplicationData.Current.LocalCacheFolder.Path);
    private readonly LazySlim<string> lazyFamilyName = new(() => Package.Current.Id.FamilyName);

    private readonly LazySlim<bool> lazyToastAvailable = new(() =>
    {
        ITaskContext taskContext = Ioc.Default.GetRequiredService<ITaskContext>();
        return taskContext.InvokeOnMainThread(() => AppNotificationManager.Default.Setting is AppNotificationSetting.Enabled);
    });

    public RuntimeOptions()
    {
        AppLaunchTime = DateTimeOffset.UtcNow;
    }

    public Version Version { get => lazyVersionAndUserAgent.Value.Version; }

    public string UserAgent { get => lazyVersionAndUserAgent.Value.UserAgent; }

    public string DataFolder { get => lazyDataFolder.Value; }

    public string LocalCache { get => lazyLocalCache.Value; }

    public string FamilyName { get => lazyFamilyName.Value; }

    public string DeviceId { get => lazyDeviceId.Value; }

    public string WebView2Version { get => lazyWebViewEnvironment.Value.Version; }

    public bool IsWebView2Supported { get => lazyWebViewEnvironment.Value.Supported; }

    public bool IsElevated { get => lazyElevated.Value; }

    public bool IsToastAvailable { get => lazyToastAvailable.Value; }

    public DateTimeOffset AppLaunchTime { get; }
}