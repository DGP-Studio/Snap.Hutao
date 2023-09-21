// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Win32.Foundation;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

internal static class HotKey
{
    private const int DefaultId = 100000;

    public static bool Register(in HWND hwnd)
    {
        return RegisterHotKey(hwnd, DefaultId, default, (uint)Windows.Win32.UI.Input.KeyboardAndMouse.VIRTUAL_KEY.VK_F8);
    }

    public static bool Unregister(in HWND hwnd)
    {
        return UnregisterHotKey(hwnd, DefaultId);
    }

    public static bool OnHotKeyPressed()
    {
        return true;
    }
}