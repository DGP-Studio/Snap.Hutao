// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Win32.Foundation;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Shell;

internal static class NotifyIcon
{
    public static bool IsPromoted(IServiceProvider serviceProvider)
    {
        try
        {
            NotifyIconController notifyIconController = serviceProvider.LockAndGetRequiredService<NotifyIconController>(NotifyIconController.InitializationSyncRoot);

            // Actual version should be above 24H2 (26100), which is 26120 without UniversalApiContract.
            if (UniversalApiContract.IsPresent(WindowsVersion.Windows11Version24H2))
            {
                return notifyIconController.GetIsPromoted();
            }

            // Shell_NotifyIconGetRect can return E_FAIL in multiple cases.
            RECT iconRect = notifyIconController.GetRect();
            if (UniversalApiContract.IsPresent(WindowsVersion.Windows11))
            {
                RECT primaryRect = DisplayArea.Primary.OuterBounds.ToRECT();
                return IntersectRect(out _, in primaryRect, in iconRect);
            }

            HWND shellTrayWnd = FindWindowExW(default, default, "Shell_TrayWnd", default);
            HWND trayNotifyWnd = FindWindowExW(shellTrayWnd, default, "TrayNotifyWnd", default);
            HWND button = FindWindowExW(trayNotifyWnd, default, "Button", default);

            if (GetWindowRect(button, out RECT buttonRect))
            {
                return !EqualRect(in buttonRect, in iconRect);
            }

            return false;
        }
        catch
        {
#if DEBUG
            // Check your explorer, did you modify your default taskbar?
            throw;
#else
            return false;
#endif
        }
    }
}