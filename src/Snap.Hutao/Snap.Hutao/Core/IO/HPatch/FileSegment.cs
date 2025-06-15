// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.IO.HPatch;

internal sealed partial class FileSegment : IDisposable
{
    private readonly SafeFileHandle handle;
    private readonly bool ownsHandle;
    private readonly long offset;
    private readonly long length;
    private readonly nint gcHandle;
    private bool disposed;

    public FileSegment(SafeFileHandle handle, bool ownsHandle = true)
        : this(handle, 0L, RandomAccess.GetLength(handle), ownsHandle)
    {
    }

    public FileSegment(SafeFileHandle handle, long length, bool ownsHandle = true)
        : this(handle, 0L, length, ownsHandle)
    {
    }

    public FileSegment(SafeFileHandle handle, long offset, long length, bool ownsHandle = true)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);

        this.handle = handle;
        this.ownsHandle = ownsHandle;
        this.offset = offset;
        this.length = length;

        gcHandle = GCHandle.ToIntPtr(GCHandle.Alloc(this));
    }

    public long Length { get => length; }

    public nint Handle { get => gcHandle; }

    public unsafe bool Read(ulong position, byte* start, byte* end)
    {
        try
        {
            return RandomAccessRead.Exactly(handle, new(start, (int)(end - start)), offset + (long)position);
        }
        catch
        {
            return false;
        }
    }

    public unsafe bool Write(ulong position, byte* start, byte* end)
    {
        try
        {
            RandomAccess.Write(handle, new ReadOnlySpan<byte>(start, (int)(end - start)), offset + (long)position);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed, true))
        {
            return;
        }

        if (ownsHandle)
        {
            handle.Dispose();
        }

        GCHandle.FromIntPtr(gcHandle).Free();
    }
}