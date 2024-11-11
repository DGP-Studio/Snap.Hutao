// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Windowing;

internal static class XamlWindowLowLevelHook
{
    private static Window window = default!;
    private static HHOOK keyboard = default!;

    public static unsafe void Initialize(Window? window)
    {
        Uninitialize();

        if (window is IXamlWindowHookLowLevelKeyboardHandler)
        {
            HOOKPROC.Create(&OnLowLevelKeyboardProcedure);
            SetWindowsHookExW(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, HOOKPROC.Create(&OnLowLevelKeyboardProcedure), GetModuleHandleW("Snap.Hutao.dll"), 0U);
        }
    }

    public static void Uninitialize()
    {
        if (window is null)
        {
            return;
        }

        if (keyboard.Value != 0)
        {
            UnhookWindowsHookEx(keyboard);
        }

        window = default!;
        keyboard = default!;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe LRESULT OnLowLevelKeyboardProcedure(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode >= 0)
        {
            // WM_KEYDOWN WM_KEYUP WM_SYSKEYDOWN WM_SYSKEYUP
            if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN || wParam == WM_SYSKEYDOWN || wParam == WM_SYSKEYUP)
            {
                ((IXamlWindowHookLowLevelKeyboardHandler)window).HandleLowLevelKeyboardEvent(ref *(KBDLLHOOKSTRUCT*)lParam);
            }
        }

        return CallNextHookEx(keyboard, nCode, wParam, lParam);
    }
}