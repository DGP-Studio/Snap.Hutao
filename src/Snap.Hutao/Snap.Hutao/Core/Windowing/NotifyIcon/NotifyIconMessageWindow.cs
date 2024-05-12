﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core.Windowing.Backdrop;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Core.Windowing.NotifyIcon;

[SuppressMessage("", "SA1310")]
internal sealed class NotifyIconMessageWindow : IDisposable
{
    public const uint WM_NOTIFYICON_CALLBACK = 0x444U;
    private const string WindowClassName = "SnapHutaoNotifyIconMessageWindowClass";

    private static readonly ConcurrentDictionary<HWND, NotifyIconMessageWindow> WindowTable = [];

    [SuppressMessage("", "SA1306")]
    private readonly uint WM_TASKBARCREATED;

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

    public Action<NotifyIconMessageWindow>? TaskbarCreated { get; set; }

    public Action<NotifyIconMessageWindow, PointUInt16>? ContextMenuRequested { get; set; }

    public HWND HWND { get; }

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
        if (!WindowTable.TryGetValue(hwnd, out NotifyIconMessageWindow? window))
        {
            return DefWindowProcW(hwnd, uMsg, wParam, lParam);
        }

        if (uMsg == window.WM_TASKBARCREATED)
        {
            // TODO: Re-add the notify icon.
            window.TaskbarCreated?.Invoke(window);
        }

        // https://learn.microsoft.com/zh-cn/windows/win32/api/shellapi/ns-shellapi-notifyicondataw
        if (uMsg is WM_NOTIFYICON_CALLBACK)
        {
            LPARAM2 lParam2 = *(LPARAM2*)&lParam;
            PointUInt16 wParam2 = *(PointUInt16*)&wParam;

            switch (lParam2.Low)
            {
                case WM_MOUSEMOVE:
                    // X: wParam2.X Y: wParam2.Y Low: WM_MOUSEMOVE
                    break;
                case NIN_SELECT:
                    // X: wParam2.X Y: wParam2.Y Low: NIN_SELECT
                    break;
                case NIN_POPUPOPEN:
                    // X: wParam2.X Y: 0? Low: NIN_POPUPOPEN
                    break;
                case NIN_POPUPCLOSE:
                    // X: wParam2.X Y: 0? Low: NIN_POPUPCLOSE
                    break;
                case WM_LBUTTONDOWN:
                case WM_LBUTTONUP:
                    break;
                case WM_RBUTTONDOWN:
                case WM_RBUTTONUP:
                    break;
                case WM_CONTEXTMENU:
                    window.ContextMenuRequested?.Invoke(window, wParam2);
                    Debug.WriteLine($"[uMsg: 0x{uMsg:X8}] [X: {wParam2.X} Y: {wParam2.Y}] [Low: WM_CONTEXTMENU High: 0x{lParam2.High:X8}]");
                    break;
                default:
                    Debug.WriteLine($"[uMsg: 0x{uMsg:X8}] [X: {wParam2.X} Y: {wParam2.Y}] [Low: 0x{lParam2.Low:X8} High: 0x{lParam2.High:X8}]");
                    break;
            }
        }
        else
        {
            switch (uMsg)
            {
                case WM_ACTIVATEAPP:
                    break;
                case WM_DWMNCRENDERINGCHANGED:
                    break;
                default:
                    Debug.WriteLine($"[uMsg: 0x{uMsg:X8}] [wParam: 0x{wParam.Value:X8}] [lParam: 0x{lParam.Value:X8}]");
                    break;
            }
        }

        return DefWindowProcW(hwnd, uMsg, wParam, lParam);
    }

    private readonly struct LPARAM2
    {
        public readonly uint Low;
        public readonly uint High;
    }
}