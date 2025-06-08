// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Win32.Foundation;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.IO.HPatch;

internal static unsafe class HPatch
{
    public static bool Patch(SafeFileHandle source, SafeFileHandle diff, ulong targetSize, SafeFileHandle target)
    {
        StreamInput sourceAdapter = new(source);
        StreamInput diffAdapter = new(diff);
        StreamOutput targetAdapter = new(target, targetSize);

        return Patch(&sourceAdapter, &diffAdapter, &targetAdapter);
    }

    public static bool Patch(SafeFileHandle source, SafeFileHandle diff, SafeFileHandle target)
    {
        StreamInput sourceAdapter = new(source);
        StreamInput diffAdapter = new(diff);
        ulong newDataSize;
        if (!NewDataSize(&diffAdapter, &newDataSize))
        {
            return false;
        }

        StreamOutput targetAdapter = new(target, newDataSize);

        return Patch(&sourceAdapter, &diffAdapter, &targetAdapter);
    }

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport("Snap.HPatch.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern BOOL NewDataSize(StreamInput* diff, ulong* pSize);

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport("Snap.HPatch.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern BOOL Patch(StreamInput* source, StreamInput* diff, StreamOutput* target);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static BOOL StreamRead(void* input, ulong position, byte* start, byte* end)
    {
        try
        {
            bool result = RandomAccessRead.Exactly(new(((StreamInput*)input)->Handle, ownsHandle: false), new(start, (int)(end - start)), (long)position);
            return result;
        }
        catch
        {
            return false;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static BOOL StreamWrite(void* output, ulong position, byte* start, byte* end)
    {
        try
        {
            RandomAccess.Write(new(((StreamOutput*)output)->Handle, ownsHandle: false), new ReadOnlySpan<byte>(start, (int)(end - start)), (long)position);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // ReSharper disable NotAccessedField.Local
    private readonly struct StreamInput
    {
#pragma warning disable CS0169
#pragma warning disable CA1823
        public readonly nint Handle;
        public readonly ulong Length;
        public readonly delegate* unmanaged[Cdecl]<void*, ulong, byte*, byte*, BOOL> Read;
        private readonly void* reserved;
#pragma warning restore CA1823
#pragma warning restore CS0169

        public StreamInput(SafeFileHandle handle)
        {
            Handle = handle.DangerousGetHandle();
            Length = (ulong)RandomAccess.GetLength(handle);
            Read = &StreamRead;
        }
    }

    private readonly struct StreamOutput
    {
#pragma warning disable CS0169
#pragma warning disable CS0649
        public readonly nint Handle;
        public readonly ulong Length;
        public readonly delegate* unmanaged[Cdecl]<void*, ulong, byte*, byte*, BOOL> Read;
        public readonly delegate* unmanaged[Cdecl]<void*, ulong, byte*, byte*, BOOL> Write;
#pragma warning restore CS0649
#pragma warning restore CS0169

        public StreamOutput(SafeFileHandle handle, ulong length)
        {
            Handle = handle.DangerousGetHandle();
            Length = length;
            Read = &StreamRead;
            Read = &StreamWrite;
        }
    }
}