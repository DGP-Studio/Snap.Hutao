// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

// ReSharper disable InconsistentNaming
[Flags]
internal enum KBDLLHOOKSTRUCT_FLAGS : uint
{
    LLKHF_EXTENDED = 1U,
    LLKHF_ALTDOWN = 0x20U,
    LLKHF_UP = 0x80U,
    LLKHF_INJECTED = 0x10U,
    LLKHF_LOWER_IL_INJECTED = 2U,
}