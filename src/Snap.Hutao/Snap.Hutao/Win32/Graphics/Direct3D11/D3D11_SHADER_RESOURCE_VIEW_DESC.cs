// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_SHADER_RESOURCE_VIEW_DESC
{
    public DXGI_FORMAT Format;
    public D3D_SRV_DIMENSION ViewDimension;
    public Union Anonymous;

    [StructLayout(LayoutKind.Explicit)]
    internal struct Union
    {
        [FieldOffset(0)]
        public D3D11_BUFFER_SRV Buffer;

        [FieldOffset(0)]
        public D3D11_TEX1D_SRV Texture1D;

        [FieldOffset(0)]
        public D3D11_TEX1D_ARRAY_SRV Texture1DArray;

        [FieldOffset(0)]
        public D3D11_TEX2D_SRV Texture2D;

        [FieldOffset(0)]
        public D3D11_TEX2D_ARRAY_SRV Texture2DArray;

        [FieldOffset(0)]
        public D3D11_TEX2DMS_SRV Texture2DMS;

        [FieldOffset(0)]
        public D3D11_TEX2DMS_ARRAY_SRV Texture2DMSArray;

        [FieldOffset(0)]
        public D3D11_TEX3D_SRV Texture3D;

        [FieldOffset(0)]
        public D3D11_TEXCUBE_SRV TextureCube;

        [FieldOffset(0)]
        public D3D11_TEXCUBE_ARRAY_SRV TextureCubeArray;

        [FieldOffset(0)]
        public D3D11_BUFFEREX_SRV BufferEx;
    }
}