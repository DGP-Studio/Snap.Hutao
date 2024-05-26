// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using Snap.Hutao.Win32.Graphics.Gdi;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal struct DXGI_OUTPUT_DESC
{
    public unsafe fixed char DeviceName[32];
    public RECT DesktopCoordinates;
    public BOOL AttachedToDesktop;
    public DXGI_MODE_ROTATION Rotation;
    public HMONITOR Monitor;
}