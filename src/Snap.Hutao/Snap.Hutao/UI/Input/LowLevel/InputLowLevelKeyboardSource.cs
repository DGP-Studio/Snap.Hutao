// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Input.LowLevel;

internal delegate void InputLowLevelKeyboardSourceEventHandler(LowLevelKeyEventArgs args);

internal static class InputLowLevelKeyboardSource
{
    private static readonly Lock syncRoot = new();
    private static HHOOK keyboard;
    private static int refCount;

    public static event InputLowLevelKeyboardSourceEventHandler? KeyDown;

    public static event InputLowLevelKeyboardSourceEventHandler? KeyUp;

    public static event InputLowLevelKeyboardSourceEventHandler? SystemKeyDown;

    public static event InputLowLevelKeyboardSourceEventHandler? SystemKeyUp;

    public static unsafe void Initialize()
    {
        Interlocked.Increment(ref refCount);
        lock (syncRoot)
        {
            if (keyboard.Value is not 0)
            {
                return;
            }

            HOOKPROC.Create(&OnLowLevelKeyboardProcedure);
            SetWindowsHookExW(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, HOOKPROC.Create(&OnLowLevelKeyboardProcedure), GetModuleHandleW("Snap.Hutao.dll"), 0U);
        }
    }

    public static void Uninitialize()
    {
        if (Interlocked.Decrement(ref refCount) is not 0)
        {
            return;
        }

        lock (syncRoot)
        {
            if (keyboard.Value is not 0)
            {
                UnhookWindowsHookEx(keyboard);
            }

            keyboard = default!;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe LRESULT OnLowLevelKeyboardProcedure(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode >= 0)
        {
            LowLevelKeyEventArgs args = new(*(KBDLLHOOKSTRUCT*)lParam);
            switch ((uint)wParam)
            {
                case WM_KEYDOWN:
                    KeyDown?.Invoke(args);
                    break;
                case WM_KEYUP:
                    KeyUp?.Invoke(args);
                    break;
                case WM_SYSKEYDOWN:
                    SystemKeyDown?.Invoke(args);
                    break;
                case WM_SYSKEYUP:
                    SystemKeyUp?.Invoke(args);
                    break;
            }

            if (args.Handled)
            {
                return BOOL.TRUE;
            }
        }

        return CallNextHookEx(keyboard, nCode, wParam, lParam);
    }
}