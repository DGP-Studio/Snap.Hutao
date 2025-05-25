// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.SystemServices;

// ReSharper disable InconsistentNaming
[Flags]
internal enum MODIFIERKEYS_FLAGS : uint
{
    MK_LBUTTON = 1u,
    MK_RBUTTON = 2u,
    MK_SHIFT = 4u,
    MK_CONTROL = 8u,
    MK_MBUTTON = 0x10u,
    MK_XBUTTON1 = 0x20u,
    MK_XBUTTON2 = 0x40u,
}