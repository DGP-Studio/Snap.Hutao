// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Input.HotKey;

internal sealed partial class HotKeyMessageWindow : IDisposable
{
    private const string WindowClassName = "SnapHutaoHotKeyMessageWindowClass";

    private static readonly ConcurrentDictionary<HWND, HotKeyMessageWindow> WindowTable = [];

    private bool isDisposed;

    private unsafe HotKeyMessageWindow()
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

        Hwnd = CreateWindowExW(0, WindowClassName, WindowClassName, 0, 0, 0, 0, 0, default, default, default, default);

        if (Hwnd == default)
        {
            Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
        }

        WindowTable.TryAdd(Hwnd, this);
    }

    ~HotKeyMessageWindow()
    {
        Dispose();
    }

    public HWND Hwnd { get; }

    private Action<HotKeyParameter>? HotKeyPressed { get; set; }

    public static HotKeyMessageWindow Create(Action<HotKeyParameter>? hotKeyPressed)
    {
        return new()
        {
            HotKeyPressed = hotKeyPressed,
        };
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;

        HotKeyPressed = null;
        DestroyWindow(Hwnd);
        WindowTable.TryRemove(Hwnd, out _);

        GC.SuppressFinalize(this);
    }

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
        }

        return DefWindowProcW(hwnd, uMsg, wParam, lParam);
    }
}