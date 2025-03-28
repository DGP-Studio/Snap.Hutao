// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionOverlayHandler : ILaunchExecutionDelegateHandler
{
    private static uint processId;
    private static LaunchExecutionOverlayWindow? window;
    private static uint lastEventTime;

    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (HutaoRuntime.IsProcessElevated)
        {
            await context.TaskContext.SwitchToMainThreadAsync();
            processId = (uint)context.Process.Id;
            window = context.ServiceProvider.GetRequiredService<LaunchExecutionOverlayWindow>();
            HWINEVENTHOOK foregroundHook = HookGameWindowEvent();

            await next().ConfigureAwait(false);

            await context.TaskContext.SwitchToMainThreadAsync();
            UnhookWinEvent(foregroundHook);
            window.Close();
            processId = 0u;
            window = default;
        }
        else
        {
            await next().ConfigureAwait(false);
        }
    }

    private static unsafe HWINEVENTHOOK HookGameWindowEvent()
    {
        return SetWinEventHook(
            WINEVENT_ID.EVENT_SYSTEM_FOREGROUND,
            WINEVENT_ID.EVENT_SYSTEM_FOREGROUND,
            default,
            WINEVENTPROC.Create(&WinEventProc),
            0,
            0,
            WINEVENT_FLAGS.WINEVENT_OUTOFCONTEXT | WINEVENT_FLAGS.WINEVENT_SKIPOWNPROCESS);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void WinEventProc(HWINEVENTHOOK hWinEventHook, WINEVENT_ID eventType, HWND hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (processId is 0u || window is null)
        {
            return;
        }

        if (dwmsEventTime < lastEventTime)
        {
            return;
        }

        GetWindowThreadProcessId(hwnd, out uint pid);
        if (pid is 0u)
        {
            return;
        }

        if (pid == processId)
        {
            window.AppWindow.Show();
        }
        else
        {
            HWND taskSwitcherHwnd = FindWindowExW(default, default, "XamlExplorerHostIslandWindow", default);
            if (taskSwitcherHwnd == hwnd)
            {
                return;
            }

            window.AppWindow.Hide();
        }

        lastEventTime = dwmsEventTime;
    }
}