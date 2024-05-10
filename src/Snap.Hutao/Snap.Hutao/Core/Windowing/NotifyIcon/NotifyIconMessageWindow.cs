// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Core.Windowing.NotifyIcon;

[SuppressMessage("", "SA1310")]
internal sealed class NotifyIconMessageWindow : IDisposable
{
    public const uint WM_NOTIFYICON_CALLBACK = 0x444U;
    private const string WindowClassName = "SnapHutaoNotifyIconMessageWindowClass";

    private static readonly ConcurrentDictionary<HWND, NotifyIconMessageWindow> WindowTable = [];

    public readonly HWND HWND;

    [SuppressMessage("", "SA1306")]
    private uint WM_TASKBARCREATED;

    private bool isDisposed;

    public unsafe NotifyIconMessageWindow()
    {
        ushort atom;
        fixed (char* className = WindowClassName)
        {
            WNDCLASSW wc = new()
            {
                lpfnWndProc = WNDPROC.Create(&OnWindowProcedure),
                lpszClassName = className,
            };

            atom = RegisterClassW(&wc);
        }

        ArgumentOutOfRangeException.ThrowIfEqual<ushort>(atom, 0);

        // https://learn.microsoft.com/zh,cn/windows/win32/shell/taskbar#taskbar,creation,notification
        WM_TASKBARCREATED = RegisterWindowMessageW("TaskbarCreated");

        HWND = CreateWindowExW(0, WindowClassName, WindowClassName, 0, 0, 0, 0, 0, default, default, default, default);

        if (HWND == default)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
        }

        WindowTable.TryAdd(HWND, this);
    }

    ~NotifyIconMessageWindow()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;

        DestroyWindow(HWND);
        WindowTable.TryRemove(HWND, out _);

        GC.SuppressFinalize(this);
    }

    [SuppressMessage("", "SH002")]
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe LRESULT OnWindowProcedure(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam)
    {
        if (WindowTable.TryGetValue(hwnd, out NotifyIconMessageWindow? window))
        {
            if (uMsg == window.WM_TASKBARCREATED)
            {
                // TODO: Re-add the notify icon.
            }

            // https://learn.microsoft.com/zh-cn/windows/win32/api/shellapi/ns-shellapi-notifyicondataw
            if (uMsg == WM_NOTIFYICON_CALLBACK)
            {
                LPARAM2 lParam2 = *(LPARAM2*)&lParam;
                WPARAM2 wParam2 = *(WPARAM2*)&wParam;
                Debug.WriteLine($"[uMsg: 0x{uMsg:X8}] [X: {wParam2.X} Y: {wParam2.Y}] [Low: 0x{lParam2.Low:X8} High: 0x{lParam2.High:X8}]");
                switch (lParam.Value)
                {

                }
            }
        }

        return DefWindowProcW(hwnd, uMsg, wParam, lParam);
    }

    private readonly struct LPARAM2
    {
        public readonly uint Low;
        public readonly uint High;
    }

    private readonly struct WPARAM2
    {
        public readonly ushort X;
        public readonly ushort Y;
    }
}