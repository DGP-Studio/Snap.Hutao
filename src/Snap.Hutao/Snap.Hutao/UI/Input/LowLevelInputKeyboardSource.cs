// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Input;

internal delegate bool LowLevelInputKeyboardSourceEventHandler(ref readonly KBDLLHOOKSTRUCT args);

internal static class LowLevelInputKeyboardSource
{
    private static HHOOK keyboard = default!;

    public static event LowLevelInputKeyboardSourceEventHandler? KeyDown;

    public static event LowLevelInputKeyboardSourceEventHandler? KeyUp;

    public static event LowLevelInputKeyboardSourceEventHandler? SystemKeyDown;

    public static event LowLevelInputKeyboardSourceEventHandler? SystemKeyUp;

    public static unsafe void Initialize()
    {
        if (keyboard.Value != 0)
        {
            return;
        }

        HOOKPROC.Create(&OnLowLevelKeyboardProcedure);
        SetWindowsHookExW(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, HOOKPROC.Create(&OnLowLevelKeyboardProcedure), GetModuleHandleW("Snap.Hutao.dll"), 0U);
    }

    public static void Uninitialize()
    {
        if (keyboard.Value != 0)
        {
            UnhookWindowsHookEx(keyboard);
        }

        keyboard = default!;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe LRESULT OnLowLevelKeyboardProcedure(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode >= 0)
        {
            ref KBDLLHOOKSTRUCT data = ref *(KBDLLHOOKSTRUCT*)lParam;
            bool handled = (uint)wParam switch
            {
                WM_KEYDOWN => KeyDown?.Invoke(in data) ?? false,
                WM_KEYUP => KeyUp?.Invoke(in data) ?? false,
                WM_SYSKEYDOWN => SystemKeyDown?.Invoke(in data) ?? false,
                WM_SYSKEYUP => SystemKeyUp?.Invoke(in data) ?? false,
                _ => false,
            };

            if (handled)
            {
                return BOOL.TRUE;
            }
        }

        return CallNextHookEx(keyboard, nCode, wParam, lParam);
    }
}