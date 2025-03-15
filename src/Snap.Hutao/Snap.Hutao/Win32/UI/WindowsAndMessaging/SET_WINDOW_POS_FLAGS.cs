// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

[Flags]
internal enum SET_WINDOW_POS_FLAGS : uint
{
    SWP_ASYNCWINDOWPOS = 0x4000u,
    SWP_DEFERERASE = 0x2000u,
    SWP_DRAWFRAME = 0x20u,
    SWP_FRAMECHANGED = 0x20u,
    SWP_HIDEWINDOW = 0x80u,
    SWP_NOACTIVATE = 0x10u,
    SWP_NOCOPYBITS = 0x100u,
    SWP_NOMOVE = 2u,
    SWP_NOOWNERZORDER = 0x200u,
    SWP_NOREDRAW = 8u,
    SWP_NOREPOSITION = 0x200u,
    SWP_NOSENDCHANGING = 0x400u,
    SWP_NOSIZE = 1u,
    SWP_NOZORDER = 4u,
    SWP_SHOWWINDOW = 0x40u
}