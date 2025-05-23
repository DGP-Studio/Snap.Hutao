// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;

namespace Snap.Hutao.UI.Input.HotKey;

internal readonly struct HotKeyParameter
{
    // DO NOT MODIFY: The size of this struct must be sizeof(LPARAM) or 8
    public readonly ushort NativeModifiers;
    public readonly VIRTUAL_KEY NativeKey;

#pragma warning disable CS0169
    private readonly int padding;
#pragma warning restore CS0169

    public HotKeyParameter(HOT_KEY_MODIFIERS modifiers, VIRTUAL_KEY key)
    {
        NativeModifiers = (ushort)modifiers;
        NativeKey = key;
    }

    public static HotKeyParameter Default
    {
        get => new(default, VIRTUAL_KEY.VK__none_);
    }

    public HOT_KEY_MODIFIERS Modifiers
    {
        get => (HOT_KEY_MODIFIERS)NativeModifiers;
    }

    public VIRTUAL_KEY Key
    {
        get => NativeKey;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NativeModifiers, NativeKey);
    }
}