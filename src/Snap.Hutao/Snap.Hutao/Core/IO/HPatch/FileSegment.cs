// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.IO.HPatch;

internal sealed partial class FileSegment : IDisposable
{
    private readonly SafeFileHandle fileHandle;
    private readonly bool ownsHandle;
    private readonly long offset;
    private bool disposed;

    public FileSegment(SafeFileHandle fileHandle, bool ownsHandle = true)
        : this(fileHandle, 0L, RandomAccess.GetLength(fileHandle), ownsHandle)
    {
    }

    public FileSegment(SafeFileHandle fileHandle, long length, bool ownsHandle = true)
        : this(fileHandle, 0L, length, ownsHandle)
    {
    }

    public FileSegment(SafeFileHandle fileHandle, long offset, long length, bool ownsHandle = true)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);

        this.fileHandle = fileHandle;
        this.ownsHandle = ownsHandle;
        this.offset = offset;
        this.Length = length;

        Handle = GCHandle.ToIntPtr(GCHandle.Alloc(this));
    }

    public long Length { get; }

    public nint Handle { get; }

    public unsafe bool Read(ulong position, byte* start, byte* end)
    {
        try
        {
            return RandomAccessRead.Exactly(fileHandle, new(start, (int)(end - start)), offset + (long)position);
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
            RandomAccess.Write(fileHandle, new ReadOnlySpan<byte>(start, (int)(end - start)), offset + (long)position);
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
            fileHandle.Dispose();
        }

        GCHandle.FromIntPtr(Handle).Free();
    }
}