// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

[Flags]
internal enum WNDCLASS_STYLES : uint
{
    CS_VREDRAW = 0x1U,
    CS_HREDRAW = 0x2U,
    CS_DBLCLKS = 0x8U,
    CS_OWNDC = 0x20U,
    CS_CLASSDC = 0x40U,
    CS_PARENTDC = 0x80U,
    CS_NOCLOSE = 0x200U,
    CS_SAVEBITS = 0x800U,
    CS_BYTEALIGNCLIENT = 0x1000U,
    CS_BYTEALIGNWINDOW = 0x2000U,
    CS_GLOBALCLASS = 0x4000U,
    CS_IME = 0x10000U,
    CS_DROPSHADOW = 0x20000U,
}