// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Accessibility;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionOverlayHandler : ILaunchExecutionDelegateHandler
{
    private static readonly WeakReference<Process> WeakProcess = new(default!);
    private static readonly WeakReference<LaunchExecutionOverlayWindow> WeakWindow = new(default!);
    private static uint lastEventTime;

    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (XamlApplicationLifetime.LaunchedWithNotifyIcon && HutaoRuntime.IsProcessElevated && context.Options.UsingOverlay)
        {
            await context.TaskContext.SwitchToMainThreadAsync();

            LaunchExecutionOverlayWindow window = context.ServiceProvider.GetRequiredService<LaunchExecutionOverlayWindow>();
            WeakWindow.SetTarget(window);
            WeakProcess.SetTarget(context.Process);

            HWINEVENTHOOK foregroundHook = default;
            try
            {
                unsafe
                {
                    foregroundHook = SetWinEventHook(
                        EVENT_SYSTEM_FOREGROUND,
                        EVENT_SYSTEM_FOREGROUND,
                        default,
                        WINEVENTPROC.Create(&WinEventProcedure),
                        0,
                        0,
                        WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
                }

                await next().ConfigureAwait(false);
            }
            finally
            {
                UnhookWinEvent(foregroundHook);
            }

            await context.TaskContext.SwitchToMainThreadAsync();
            window.PreventClose = false;
            window.Close();
        }
        else
        {
            await next().ConfigureAwait(false);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void WinEventProcedure(HWINEVENTHOOK hWinEventHook, uint @event, HWND hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        if (!WeakProcess.TryGetTarget(out Process? process) || !WeakWindow.TryGetTarget(out LaunchExecutionOverlayWindow? window))
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

        if (!process.IsRunning())
        {
            return;
        }

        if (window.HideByHotKey)
        {
            return;
        }

        if (pid == (uint)process.Id)
        {
            window.AppWindow.Show(false);
            window.HideByEvent = false;
        }
        else
        {
            if (GetForegroundWindow() == process.MainWindowHandle)
            {
                return;
            }

            window.AppWindow.Hide();
            window.HideByEvent = true;
        }

        lastEventTime = dwmsEventTime;
    }
}