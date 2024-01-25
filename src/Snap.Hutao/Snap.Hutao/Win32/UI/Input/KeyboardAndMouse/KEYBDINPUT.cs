// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;

[SuppressMessage("", "SA1307")]
internal struct KEYBDINPUT
{
    public VIRTUAL_KEY wVk;
    public ushort wScan;
    public KEYBD_EVENT_FLAGS dwFlags;
    public uint time;
    public nuint dwExtraInfo;
}