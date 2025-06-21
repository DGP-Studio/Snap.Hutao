// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.IO.HPatch;

internal readonly unsafe struct StreamInput
{
#pragma warning disable CS0169
#pragma warning disable CA1823
    public readonly nint Handle;
    public readonly ulong Length;
    public readonly delegate* unmanaged[Cdecl]<void*, ulong, byte*, byte*, BOOL> Read;
    private readonly void* reserved;
#pragma warning restore CA1823
#pragma warning restore CS0169

    public StreamInput(FileSegment file)
    {
        Handle = file.Handle;
        Length = (ulong)file.Length;
        Read = &StreamIO.FileSegmentRead;
    }

    public StreamInput(Stream stream)
    {
        Verify.Operation(stream.CanSeek, "Input stream must support seeking.");
        Handle = GCHandle.ToIntPtr(GCHandle.Alloc(stream));
        Read = &StreamIO.StreamRead;
    }
}