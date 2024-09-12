// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppNotifications;

namespace Snap.Hutao.Service.Notification;

internal sealed partial class ToastNotificationLifeTime : IToastNotificationLifeTime
{
    /// <inheritdoc/>
    public void Dispose()
    {
        // 用于在程序退出时尝试清除所有的系统通知
        try
        {
            AppNotificationManager.Default.RemoveAllAsync().AsTask().GetAwaiter().GetResult();
            AppNotificationManager.Default.Unregister();
        }
        catch
        {
        }
    }
}