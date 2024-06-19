// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace Snap.Hutao.Extension;

internal static class AppNotificationBuilderExtension
{
    /// <summary>
    /// Build and show the notification
    /// </summary>
    /// <param name="builder">this</param>
    /// <param name="manager">Defaults to <see cref="AppNotificationManager.Default"/></param>
    public static void Show(this AppNotificationBuilder builder, AppNotificationManager? manager = default)
    {
        (manager ?? AppNotificationManager.Default).Show(builder.BuildNotification());
    }
}