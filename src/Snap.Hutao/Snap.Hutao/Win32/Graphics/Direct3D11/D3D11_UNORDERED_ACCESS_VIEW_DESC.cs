// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_UNORDERED_ACCESS_VIEW_DESC
{
    public DXGI_FORMAT Format;
    public D3D11_UAV_DIMENSION ViewDimension;
    public Union Anonymous;

    [StructLayout(LayoutKind.Explicit)]
    internal struct Union
    {
        [FieldOffset(0)]
        public D3D11_BUFFER_UAV Buffer;

        [FieldOffset(0)]
        public D3D11_TEX1D_UAV Texture1D;

        [FieldOffset(0)]
        public D3D11_TEX1D_ARRAY_UAV Texture1DArray;

        [FieldOffset(0)]
        public D3D11_TEX2D_UAV Texture2D;

        [FieldOffset(0)]
        public D3D11_TEX2D_ARRAY_UAV Texture2DArray;

        [FieldOffset(0)]
        public D3D11_TEX3D_UAV Texture3D;
    }
}