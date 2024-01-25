// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

[SuppressMessage("", "SA1202")]
[SuppressMessage("", "SA1307")]
internal struct WINDOWPLACEMENT
{
    public uint length;
    public WINDOWPLACEMENT_FLAGS flags;
    private uint showCmd;
    public POINT ptMinPosition;
    public POINT ptMaxPosition;
    public RECT rcNormalPosition;

    public SHOW_WINDOW_CMD ShowCmd
    {
        readonly get => (SHOW_WINDOW_CMD)showCmd;
        set => showCmd = (uint)value;
    }

    public static unsafe WINDOWPLACEMENT Create()
    {
        return new() { length = (uint)sizeof(WINDOWPLACEMENT) };
    }
}