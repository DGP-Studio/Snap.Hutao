// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core;

[Service(ServiceLifetime.Singleton)]
internal sealed class RuntimeOptions
{
    [Obsolete("This property only exist for binding purpose")]
    public Version Version { get => HutaoRuntime.Version; }

    [Obsolete("This property only exist for binding purpose")]
    public string DataFolder { get => HutaoRuntime.DataDirectory; }

    [Obsolete("This property only exist for binding purpose")]
    public string DeviceId { get => HutaoRuntime.DeviceId; }

    [Obsolete("This property only exist for binding purpose")]
    public string WebView2Version { get => HutaoRuntime.WebView2Version.Version; }

    [Obsolete("This property only exist for binding purpose")]
    public bool IsToastAvailable { get => HutaoRuntime.IsAppNotificationEnabled; }
}