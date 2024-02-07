// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;

[SuppressMessage("", "SA1307")]
internal struct MOUSEINPUT
{
    public int dx;
    public int dy;
    public uint mouseData;
    public MOUSE_EVENT_FLAGS dwFlags;
    public uint time;
    public nuint dwExtraInfo;
}