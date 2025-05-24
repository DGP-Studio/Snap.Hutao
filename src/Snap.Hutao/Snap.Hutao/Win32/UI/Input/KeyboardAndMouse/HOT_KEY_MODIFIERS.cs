// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;

// ReSharper disable InconsistentNaming
[Flags]
internal enum HOT_KEY_MODIFIERS : uint
{
    MOD_ALT = 1u,
    MOD_CONTROL = 2u,
    MOD_NOREPEAT = 0x4000u,
    MOD_SHIFT = 4u,
    MOD_WIN = 8u,
}