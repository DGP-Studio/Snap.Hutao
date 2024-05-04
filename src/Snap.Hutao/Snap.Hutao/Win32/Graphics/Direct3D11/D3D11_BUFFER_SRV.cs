// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal struct D3D11_BUFFER_SRV
{
    public Union1 Anonymous1;
    public Union2 Anonymous2;

    [StructLayout(LayoutKind.Explicit)]
    internal struct Union1
    {
        [FieldOffset(0)]
        public uint FirstElement;

        [FieldOffset(0)]
        public uint ElementOffset;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct Union2
    {
        [FieldOffset(0)]
        public uint NumElements;

        [FieldOffset(0)]
        public uint ElementWidth;
    }
}