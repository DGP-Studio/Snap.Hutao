// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;

namespace Snap.Hutao.Service.Notification;

/// <summary>
/// 系统通知生命周期时间
/// </summary>
internal sealed class ToastNotificationLifeTime : IToastNotificationLifeTime
{
    /// <inheritdoc/>
    public void Dispose()
    {
        // 用于在程序退出时尝试清除所有的系统通知
        try
        {
            ToastNotificationManagerCompat.History.Clear();
        }
        catch
        {
        }
    }
}