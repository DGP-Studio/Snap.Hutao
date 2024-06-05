// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

[SuppressMessage("", "SA1307")]
internal struct DXGI_PRESENT_PARAMETERS
{
    public uint DirtyRectsCount;
    public unsafe RECT* pDirtyRects;
    public unsafe RECT* pScrollRect;
    public unsafe POINT* pScrollOffset;
}