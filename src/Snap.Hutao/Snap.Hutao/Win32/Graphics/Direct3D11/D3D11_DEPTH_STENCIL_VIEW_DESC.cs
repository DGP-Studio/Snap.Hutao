// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_DEPTH_STENCIL_VIEW_DESC
{
    public DXGI_FORMAT Format;
    public D3D11_DSV_DIMENSION ViewDimension;
    public uint Flags;
    public Union Anonymous;

    [StructLayout(LayoutKind.Explicit)]
    public struct Union
    {
        [FieldOffset(0)]
        public D3D11_TEX1D_DSV Texture1D;

        [FieldOffset(0)]
        public D3D11_TEX1D_ARRAY_DSV Texture1DArray;

        [FieldOffset(0)]
        public D3D11_TEX2D_DSV Texture2D;

        [FieldOffset(0)]
        public D3D11_TEX2D_ARRAY_DSV Texture2DArray;

        [FieldOffset(0)]
        public D3D11_TEX2DMS_DSV Texture2DMS;

        [FieldOffset(0)]
        public D3D11_TEX2DMS_ARRAY_DSV Texture2DMSArray;
    }
}