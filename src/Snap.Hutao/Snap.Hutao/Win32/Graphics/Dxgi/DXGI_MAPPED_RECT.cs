// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal struct DXGI_MAPPED_RECT
{
    public int Pitch;
    public unsafe byte* pBits;
}