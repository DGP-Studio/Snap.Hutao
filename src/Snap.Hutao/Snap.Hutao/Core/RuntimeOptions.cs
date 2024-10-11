// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core;

[Injection(InjectAs.Singleton)]
internal sealed class RuntimeOptions
{
    [Obsolete]
    public Version Version { get => HutaoRuntime.Version; }

    [Obsolete]
    public string UserAgent { get => HutaoRuntime.UserAgent; }

    [Obsolete]
    public string DataFolder { get => HutaoRuntime.DataFolder; }

    [Obsolete]
    public string LocalCache { get => HutaoRuntime.LocalCache; }

    [Obsolete]
    public string FamilyName { get => HutaoRuntime.FamilyName; }

    [Obsolete]
    public string DeviceId { get => HutaoRuntime.DeviceId; }

    [Obsolete]
    public string WebView2Version { get => HutaoRuntime.WebView2Version.Version; }

    [Obsolete]
    public bool IsWebView2Supported { get => HutaoRuntime.WebView2Version.Supported; }

    [Obsolete]
    public bool IsElevated { get => HutaoRuntime.IsProcessElevated; }

    [Obsolete]
    public bool IsToastAvailable { get => HutaoRuntime.IsAppNotificationEnabled; }

    [Obsolete]
    public DateTimeOffset AppLaunchTime { get => HutaoRuntime.LaunchTime; }
}