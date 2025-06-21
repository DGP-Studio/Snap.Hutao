// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.IO.HPatch;

internal static unsafe class HPatch
{
    public static long GetNewDataSize(FileSegment diff)
    {
        StreamInput diffAdapter = new(diff);
        ulong size = 0;
        NewDataSize(&diffAdapter, &size);
        return (long)size;
    }

    public static bool Patch(FileSegment source, FileSegment diff, FileSegment target)
    {
        StreamInput sourceAdapter = new(source);
        StreamInput diffAdapter = new(diff);
        StreamOutput targetAdapter = new(target);

        return Patch(&sourceAdapter, &diffAdapter, &targetAdapter);
    }

    public static bool PatchZstandard(FileSegment source, FileSegment diff, FileSegment target)
    {
        StreamInput sourceAdapter = new(source);
        StreamInput diffAdapter = new(diff);
        StreamOutput targetAdapter = new(target);
        Decompress decompressor = Decompress.CreateZstandard();

        return PatchWithDecompressor(&sourceAdapter, &diffAdapter, &targetAdapter, &decompressor);
    }

    public static bool Patch(FileSegment source, FileSegment diff, Stream target)
    {
        StreamInput sourceAdapter = new(source);
        StreamInput diffAdapter = new(diff);
        StreamOutput targetAdapter = new(target);

        try
        {
            return Patch(&sourceAdapter, &diffAdapter, &targetAdapter);
        }
        finally
        {
            GCHandle.FromIntPtr(targetAdapter.Handle).Free();
        }
    }

    public static bool PatchZstandard(FileSegment source, FileSegment diff, Stream target)
    {
        StreamInput sourceAdapter = new(source);
        StreamInput diffAdapter = new(diff);
        StreamOutput targetAdapter = new(target);
        Decompress decompressor = Decompress.CreateZstandard();

        try
        {
            return PatchWithDecompressor(&sourceAdapter, &diffAdapter, &targetAdapter, &decompressor);
        }
        finally
        {
            GCHandle.FromIntPtr(targetAdapter.Handle).Free();
        }
    }

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport("Snap.HPatch.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern BOOL NewDataSize(StreamInput* diff, ulong* pSize);

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport("Snap.HPatch.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern BOOL Patch(StreamInput* source, StreamInput* diff, StreamOutput* target);

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport("Snap.HPatch.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern BOOL PatchWithDecompressor(StreamInput* source, StreamInput* diff, StreamOutput* target, Decompress* decompressor);
}