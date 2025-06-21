// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.IO.HPatch;

internal static unsafe class StreamIO
{
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static BOOL FileSegmentRead(void* input, ulong position, byte* start, byte* end)
    {
        return GCHandle.FromIntPtr(((StreamInput*)input)->Handle).Target is FileSegment file && file.Read(position, start, end);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static BOOL StreamRead(void* input, ulong position, byte* start, byte* end)
    {
        if (GCHandle.FromIntPtr(((StreamInput*)input)->Handle).Target is not Stream stream)
        {
            return false;
        }

        try
        {
            stream.Seek((long)position, SeekOrigin.Begin);
            stream.ReadExactly(new(start, (int)(end - start)));
            return true;
        }
        catch
        {
            return false;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static BOOL FileSegmentWrite(void* output, ulong position, byte* start, byte* end)
    {
        return GCHandle.FromIntPtr(((StreamInput*)output)->Handle).Target is FileSegment file && file.Write(position, start, end);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static BOOL StreamWrite(void* output, ulong position, byte* start, byte* end)
    {
        if (GCHandle.FromIntPtr(((StreamInput*)output)->Handle).Target is not Stream stream)
        {
            return false;
        }

        try
        {
            stream.Seek((long)position, SeekOrigin.Begin);
            stream.Write(new(start, (int)(end - start)));
            return true;
        }
        catch
        {
            return false;
        }
    }
}