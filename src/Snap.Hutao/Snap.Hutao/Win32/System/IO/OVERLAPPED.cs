// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.System.IO;

[SuppressMessage("", "SA1307")]
internal struct OVERLAPPED
{
    public nuint Internal;
    public nuint InternalHigh;
    public Union Anonymous;
    public HANDLE hEvent;

    [StructLayout(LayoutKind.Explicit)]
    internal struct Union
    {
        [FieldOffset(0)]
        public Struct Anonymous;

        [FieldOffset(0)]
        public unsafe void* Pointer;

        internal struct Struct
        {
            public uint Offset;
            public uint OffsetHigh;
        }
    }
}