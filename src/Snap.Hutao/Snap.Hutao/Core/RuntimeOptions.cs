// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core;

[Injection(InjectAs.Singleton)]
internal sealed class RuntimeOptions
{
    [Obsolete("This property only exist for binding purpose")]
    public Version Version { get => HutaoRuntime.Version; }

    [Obsolete("This property only exist for binding purpose")]
    public string UserAgent { get => HutaoRuntime.UserAgent; }

    [Obsolete("This property only exist for binding purpose")]
    public string DataFolder { get => HutaoRuntime.DataFolder; }

    [Obsolete("This property only exist for binding purpose")]
    public string LocalCache { get => HutaoRuntime.LocalCache; }

    [Obsolete("This property only exist for binding purpose")]
    public string FamilyName { get => HutaoRuntime.FamilyName; }

    [Obsolete("This property only exist for binding purpose")]
    public string DeviceId { get => HutaoRuntime.DeviceId; }

    [Obsolete("This property only exist for binding purpose")]
    public string WebView2Version { get => HutaoRuntime.WebView2Version.Version; }

    [Obsolete("This property only exist for binding purpose")]
    public bool IsWebView2Supported { get => HutaoRuntime.WebView2Version.Supported; }

    [Obsolete("This property only exist for binding purpose")]
    public bool IsElevated { get => HutaoRuntime.IsProcessElevated; }

    [Obsolete("This property only exist for binding purpose")]
    public bool IsToastAvailable { get => HutaoRuntime.IsAppNotificationEnabled; }

    [Obsolete("This property only exist for binding purpose")]
    public DateTimeOffset AppLaunchTime { get => HutaoRuntime.LaunchTime; }
}