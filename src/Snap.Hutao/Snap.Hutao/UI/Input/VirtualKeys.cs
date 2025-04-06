// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using System.Collections.Immutable;

namespace Snap.Hutao.UI.Input;

internal static class VirtualKeys
{
    public static ImmutableArray<NameValue<VIRTUAL_KEY>> Values { get; } = ImmutableCollectionsNameValue.FromEnum<VIRTUAL_KEY>();

    public static ImmutableArray<NameValue<VIRTUAL_KEY>> HotKeyValues { get; } = ImmutableCollectionsNameValue.From(RawHotKeyValues);

    private static ReadOnlySpan<VIRTUAL_KEY> RawHotKeyValues
    {
        get =>
        [
            VIRTUAL_KEY.VK_BACK,
            VIRTUAL_KEY.VK_TAB,
            VIRTUAL_KEY.VK_RETURN,
            VIRTUAL_KEY.VK_PAUSE,
            VIRTUAL_KEY.VK_CAPITAL,
            VIRTUAL_KEY.VK_SPACE,
            VIRTUAL_KEY.VK_PRIOR,
            VIRTUAL_KEY.VK_NEXT,
            VIRTUAL_KEY.VK_END,
            VIRTUAL_KEY.VK_HOME,
            VIRTUAL_KEY.VK_LEFT,
            VIRTUAL_KEY.VK_UP,
            VIRTUAL_KEY.VK_RIGHT,
            VIRTUAL_KEY.VK_DOWN,
            VIRTUAL_KEY.VK_INSERT,
            VIRTUAL_KEY.VK_DELETE,
            VIRTUAL_KEY.VK_0,
            VIRTUAL_KEY.VK_1,
            VIRTUAL_KEY.VK_2,
            VIRTUAL_KEY.VK_3,
            VIRTUAL_KEY.VK_4,
            VIRTUAL_KEY.VK_5,
            VIRTUAL_KEY.VK_6,
            VIRTUAL_KEY.VK_7,
            VIRTUAL_KEY.VK_8,
            VIRTUAL_KEY.VK_9,
            VIRTUAL_KEY.VK_A,
            VIRTUAL_KEY.VK_B,
            VIRTUAL_KEY.VK_C,
            VIRTUAL_KEY.VK_D,
            VIRTUAL_KEY.VK_E,
            VIRTUAL_KEY.VK_F,
            VIRTUAL_KEY.VK_G,
            VIRTUAL_KEY.VK_H,
            VIRTUAL_KEY.VK_I,
            VIRTUAL_KEY.VK_J,
            VIRTUAL_KEY.VK_K,
            VIRTUAL_KEY.VK_L,
            VIRTUAL_KEY.VK_M,
            VIRTUAL_KEY.VK_N,
            VIRTUAL_KEY.VK_O,
            VIRTUAL_KEY.VK_P,
            VIRTUAL_KEY.VK_Q,
            VIRTUAL_KEY.VK_R,
            VIRTUAL_KEY.VK_S,
            VIRTUAL_KEY.VK_T,
            VIRTUAL_KEY.VK_U,
            VIRTUAL_KEY.VK_V,
            VIRTUAL_KEY.VK_W,
            VIRTUAL_KEY.VK_X,
            VIRTUAL_KEY.VK_Y,
            VIRTUAL_KEY.VK_Z,
            VIRTUAL_KEY.VK_NUMPAD0,
            VIRTUAL_KEY.VK_NUMPAD1,
            VIRTUAL_KEY.VK_NUMPAD2,
            VIRTUAL_KEY.VK_NUMPAD3,
            VIRTUAL_KEY.VK_NUMPAD4,
            VIRTUAL_KEY.VK_NUMPAD5,
            VIRTUAL_KEY.VK_NUMPAD6,
            VIRTUAL_KEY.VK_NUMPAD7,
            VIRTUAL_KEY.VK_NUMPAD8,
            VIRTUAL_KEY.VK_NUMPAD9,
            VIRTUAL_KEY.VK_MULTIPLY,
            VIRTUAL_KEY.VK_ADD,
            VIRTUAL_KEY.VK_SEPARATOR,
            VIRTUAL_KEY.VK_SUBTRACT,
            VIRTUAL_KEY.VK_DECIMAL,
            VIRTUAL_KEY.VK_DIVIDE,
            VIRTUAL_KEY.VK_F1,
            VIRTUAL_KEY.VK_F2,
            VIRTUAL_KEY.VK_F3,
            VIRTUAL_KEY.VK_F4,
            VIRTUAL_KEY.VK_F5,
            VIRTUAL_KEY.VK_F6,
            VIRTUAL_KEY.VK_F7,
            VIRTUAL_KEY.VK_F8,
            VIRTUAL_KEY.VK_F9,
            VIRTUAL_KEY.VK_F10,
            VIRTUAL_KEY.VK_F11,
            VIRTUAL_KEY.VK_F12,
            VIRTUAL_KEY.VK_F13,
            VIRTUAL_KEY.VK_F14,
            VIRTUAL_KEY.VK_F15,
            VIRTUAL_KEY.VK_F16,
            VIRTUAL_KEY.VK_F17,
            VIRTUAL_KEY.VK_F18,
            VIRTUAL_KEY.VK_F19,
            VIRTUAL_KEY.VK_F20,
            VIRTUAL_KEY.VK_F21,
            VIRTUAL_KEY.VK_F22,
            VIRTUAL_KEY.VK_F23,
            VIRTUAL_KEY.VK_F24,
            VIRTUAL_KEY.VK_NUMLOCK,
            VIRTUAL_KEY.VK_SCROLL,
            VIRTUAL_KEY.VK_BROWSER_BACK,
            VIRTUAL_KEY.VK_BROWSER_REFRESH,
            VIRTUAL_KEY.VK_BROWSER_HOME,
            VIRTUAL_KEY.VK_VOLUME_MUTE,
            VIRTUAL_KEY.VK_VOLUME_DOWN,
            VIRTUAL_KEY.VK_VOLUME_UP,
            VIRTUAL_KEY.VK_MEDIA_NEXT_TRACK,
            VIRTUAL_KEY.VK_MEDIA_PREV_TRACK,
            VIRTUAL_KEY.VK_MEDIA_STOP,
            VIRTUAL_KEY.VK_MEDIA_PLAY_PAUSE,
            VIRTUAL_KEY.VK_OEM_1,
            VIRTUAL_KEY.VK_OEM_PLUS,
            VIRTUAL_KEY.VK_OEM_COMMA,
            VIRTUAL_KEY.VK_OEM_MINUS,
            VIRTUAL_KEY.VK_OEM_PERIOD,
            VIRTUAL_KEY.VK_OEM_2,
            VIRTUAL_KEY.VK_OEM_3,
            VIRTUAL_KEY.VK_OEM_4,
            VIRTUAL_KEY.VK_OEM_5,
            VIRTUAL_KEY.VK_OEM_6,
            VIRTUAL_KEY.VK_OEM_7,
            VIRTUAL_KEY.VK_OEM_8,
            VIRTUAL_KEY.VK_OEM_102,
            VIRTUAL_KEY.VK_PLAY,
            VIRTUAL_KEY.VK__none_, // VK__none_ must be the last one
        ];
    }

    public static NameValue<VIRTUAL_KEY> First(ImmutableArray<NameValue<VIRTUAL_KEY>> array, VIRTUAL_KEY value)
    {
        // The value may come from the result of a method call, so this method
        // is intentionally made to avoid multiple calls to compute input value
        return array.First(n => n.Value == value);
    }
}