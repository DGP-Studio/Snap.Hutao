// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Graphics.Dxgi.Common;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_TEXTURE3D_DESC
{
    public uint Width;
    public uint Height;
    public uint Depth;
    public uint MipLevels;
    public DXGI_FORMAT Format;
    public D3D11_USAGE Usage;
    public D3D11_BIND_FLAG BindFlags;
    public D3D11_CPU_ACCESS_FLAG CPUAccessFlags;
    public D3D11_RESOURCE_MISC_FLAG MiscFlags;
}