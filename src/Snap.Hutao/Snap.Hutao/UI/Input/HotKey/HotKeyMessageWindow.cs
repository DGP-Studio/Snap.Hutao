// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.User32;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.UI.Input.HotKey;

internal sealed class HotKeyMessageWindow : IDisposable
{
    private const string WindowClassName = "SnapHutaoHotKeyMessageWindowClass";

    private static readonly ConcurrentDictionary<HWND, HotKeyMessageWindow> WindowTable = [];

    private bool isDisposed;

    public unsafe HotKeyMessageWindow()
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

        ArgumentOutOfRangeException.ThrowIfZero(atom);

        HWND = CreateWindowExW(0, WindowClassName, WindowClassName, 0, 0, 0, 0, 0, default, default, default, default);

        if (HWND == default)
        {
            Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
        }

        WindowTable.TryAdd(HWND, this);
    }

    ~HotKeyMessageWindow()
    {
        Dispose();
    }

    public Action<HotKeyParameter>? HotKeyPressed { get; set; }

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
        if (!WindowTable.TryGetValue(hwnd, out HotKeyMessageWindow? window))
        {
            return DefWindowProcW(hwnd, uMsg, wParam, lParam);
        }

        switch (uMsg)
        {
            case WM_HOTKEY:
                window.HotKeyPressed?.Invoke(*(HotKeyParameter*)&lParam);
                break;
            default:
                break;
        }

        return DefWindowProcW(hwnd, uMsg, wParam, lParam);
    }
}