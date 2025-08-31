// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

// ReSharper disable InconsistentNaming
[SuppressMessage("", "SA1307")]
internal struct KBDLLHOOKSTRUCT
{
#pragma warning disable CS0649
    public uint vkCode;
    public uint scanCode;
    public KBDLLHOOKSTRUCT_FLAGS flags;
    public uint time;
    public nuint dwExtraInfo;
#pragma warning disable CS0649
}