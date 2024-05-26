// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.UI.Shell;

[SuppressMessage("", "SA1307")]
internal struct NOTIFYICONDATAW
{
    public uint cbSize;
    public HWND hWnd;
    public uint uID;
    public NOTIFY_ICON_DATA_FLAGS uFlags;
    public uint uCallbackMessage;
    public HICON hIcon;
    public unsafe fixed char szTip[128];
    public NOTIFY_ICON_STATE dwState;
    public NOTIFY_ICON_STATE dwStateMask;
    public unsafe fixed char szInfo[256];
    public Union Anonymous;
    public unsafe fixed char szInfoTitle[64];
    public NOTIFY_ICON_INFOTIP_FLAGS dwInfoFlags;
    public Guid guidItem;
    public HICON hBalloonIcon;

    [StructLayout(LayoutKind.Explicit)]
    internal struct Union
    {
        [FieldOffset(0)]
        public uint uTimeout;

        [FieldOffset(0)]
        public uint uVersion;
    }
}