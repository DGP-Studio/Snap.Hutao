// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SuppressMessage("", "SA1307")]
internal struct D3D11_MAPPED_SUBRESOURCE
{
    public unsafe void* pData;
    public uint RowPitch;
    public uint DepthPitch;
}