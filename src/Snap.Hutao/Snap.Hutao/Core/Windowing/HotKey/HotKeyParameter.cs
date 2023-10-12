// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Snap.Hutao.Core.Windowing.HotKey;

internal readonly struct HotKeyParameter
{
    public readonly ushort NativeModifier;
    public readonly VIRTUAL_KEY Key;

    public readonly HOT_KEY_MODIFIERS Modifier
    {
        get => (HOT_KEY_MODIFIERS)NativeModifier;
    }
}