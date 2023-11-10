// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.System;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Snap.Hutao.Core.Windowing.HotKey;

/// <summary>
/// HotKeyParameter
/// The size of this struct must be sizeof(LPARAM) or 4
/// </summary>
internal readonly struct HotKeyParameter : IEquatable<HotKeyCombination>
{
    public readonly ushort NativeModifiers;
    public readonly VIRTUAL_KEY NativeKey;

    public HotKeyParameter(HOT_KEY_MODIFIERS modifiers, VirtualKey key)
    {
        NativeModifiers = (ushort)modifiers;
        NativeKey = (VIRTUAL_KEY)key;
    }

    public readonly HOT_KEY_MODIFIERS Modifiers
    {
        get => (HOT_KEY_MODIFIERS)NativeModifiers;
    }

    public readonly VirtualKey Key
    {
        get => (VirtualKey)NativeKey;
    }

    public bool Equals(HotKeyCombination? other)
    {
        if (other is null)
        {
            return false;
        }

        return Modifiers == other.Modifiers && Key == other.Key;
    }
}