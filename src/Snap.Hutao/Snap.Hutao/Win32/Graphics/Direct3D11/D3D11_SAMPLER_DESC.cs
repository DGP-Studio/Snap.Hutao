// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_SAMPLER_DESC
{
    public D3D11_FILTER Filter;
    public D3D11_TEXTURE_ADDRESS_MODE AddressU;
    public D3D11_TEXTURE_ADDRESS_MODE AddressV;
    public D3D11_TEXTURE_ADDRESS_MODE AddressW;
    public float MipLODBias;
    public uint MaxAnisotropy;
    public D3D11_COMPARISON_FUNC ComparisonFunc;
    public unsafe fixed float BorderColor[4];
    public float MinLOD;
    public float MaxLOD;
}