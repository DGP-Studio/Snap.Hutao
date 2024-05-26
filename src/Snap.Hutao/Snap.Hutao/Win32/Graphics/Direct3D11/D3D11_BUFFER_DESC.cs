// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_BUFFER_DESC
{
    public uint ByteWidth;
    public D3D11_USAGE Usage;
    public D3D11_BIND_FLAG BindFlags;
    public D3D11_CPU_ACCESS_FLAG CPUAccessFlags;
    public D3D11_RESOURCE_MISC_FLAG MiscFlags;
    public uint StructureByteStride;
}