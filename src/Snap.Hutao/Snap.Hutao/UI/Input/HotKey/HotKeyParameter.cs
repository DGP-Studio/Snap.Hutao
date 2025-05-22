// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;

namespace Snap.Hutao.UI.Input.HotKey;

internal readonly struct HotKeyParameter
{
    // DO NOT MODIFY: The size of this struct must be sizeof(LPARAM) or 8
    public readonly ushort NativeModifiers;
    public readonly VIRTUAL_KEY NativeKey;
    private readonly int padding;

    public HotKeyParameter(HOT_KEY_MODIFIERS modifiers, VIRTUAL_KEY key)
    {
        NativeModifiers = (ushort)modifiers;
        NativeKey = key;
    }

    public readonly HOT_KEY_MODIFIERS Modifiers
    {
        get => (HOT_KEY_MODIFIERS)NativeModifiers;
    }

    public readonly VIRTUAL_KEY Key
    {
        get => NativeKey;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NativeModifiers, NativeKey);
    }
}