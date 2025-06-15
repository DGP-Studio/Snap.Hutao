// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Win32.Foundation;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.IO.HPatch;

internal static unsafe partial class HPatch
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

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport("Snap.HPatch.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern BOOL NewDataSize(StreamInput* diff, ulong* pSize);

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport("Snap.HPatch.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern BOOL Patch(StreamInput* source, StreamInput* diff, StreamOutput* target);

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport("Snap.HPatch.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern BOOL PatchWithDecompressor(StreamInput* source, StreamInput* diff, StreamOutput* target, Decompress* decompressor);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static BOOL StreamRead(void* input, ulong position, byte* start, byte* end)
    {
        return GCHandle.FromIntPtr(((StreamInput*)input)->Handle).Target is FileSegment file && file.Read(position, start, end);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static BOOL StreamWrite(void* output, ulong position, byte* start, byte* end)
    {
        return GCHandle.FromIntPtr(((StreamInput*)output)->Handle).Target is FileSegment file && file.Write(position, start, end);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static BOOL ZstandardIsCanOpen(PCSTR compressType)
    {
        return MemoryMarshal.CreateReadOnlySpanFromNullTerminated(compressType).SequenceEqual("zstd"u8);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static nint ZstandardOpen(Decompress* decompressor, ulong dataSize, StreamInput* codeStream, ulong codeBegin, ulong codeEnd)
    {
        ZstandardDecompressStream stream = new(new StreamInputStream(codeStream, codeBegin, codeEnd));
        return GCHandle.ToIntPtr(GCHandle.Alloc(stream));
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static BOOL ZstandardClose(Decompress* decompressor, nint handle)
    {
        GCHandle gcHandle = GCHandle.FromIntPtr(handle);
        if (gcHandle.Target is not ZstandardDecompressStream stream)
        {
            return true;
        }

        stream.Dispose();
        gcHandle.Free();
        return true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static BOOL ZstandardDecompress(nint handle, byte* data, byte* dataEnd)
    {
        GCHandle gcHandle = GCHandle.FromIntPtr(handle);
        if (gcHandle.Target is not ZstandardDecompressStream stream)
        {
            return false;
        }

        try
        {
            stream.ReadExactly(new(data, (int)(dataEnd - data)));
            return true;
        }
        catch
        {
            return false;
        }
    }

    // ReSharper disable NotAccessedField.Local
    internal readonly struct StreamInput
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
            Read = &StreamRead;
        }
    }

    internal readonly struct StreamOutput
    {
#pragma warning disable CS0169
#pragma warning disable CS0649
        public readonly nint Handle;
        public readonly ulong Length;
        public readonly delegate* unmanaged[Cdecl]<void*, ulong, byte*, byte*, BOOL> Read;
        public readonly delegate* unmanaged[Cdecl]<void*, ulong, byte*, byte*, BOOL> Write;
#pragma warning restore CS0649
#pragma warning restore CS0169

        public StreamOutput(FileSegment file)
        {
            Handle = file.Handle;
            Length = (ulong)file.Length;
            Read = &StreamRead;
            Write = &StreamWrite;
        }
    }

    internal struct Decompress
    {
#pragma warning disable CS0169
        private readonly delegate* unmanaged[Cdecl]<PCSTR, BOOL> isCanOpen;
        private readonly delegate* unmanaged[Cdecl]<Decompress*, ulong, StreamInput*, ulong, ulong, nint> open;
        private readonly delegate* unmanaged[Cdecl]<Decompress*, nint, BOOL> close;
        private readonly delegate* unmanaged[Cdecl]<nint, byte*, byte*, BOOL> decompress;
        private readonly delegate* unmanaged[Cdecl]<nint, ulong, StreamInput*, ulong, ulong, BOOL> reset;
        private int error;
#pragma warning restore CS0169

        public Decompress(
            delegate* unmanaged[Cdecl]<PCSTR, BOOL> isCanOpen,
            delegate* unmanaged[Cdecl]<Decompress*, ulong, StreamInput*, ulong, ulong, nint> open,
            delegate* unmanaged[Cdecl]<Decompress*, nint, BOOL> close,
            delegate* unmanaged[Cdecl]<nint, byte*, byte*, BOOL> decompress,
            delegate* unmanaged[Cdecl]<nint, ulong, StreamInput*, ulong, ulong, BOOL> reset)
        {
            this.isCanOpen = isCanOpen;
            this.open = open;
            this.close = close;
            this.decompress = decompress;
            this.reset = reset;
        }

        public static Decompress CreateZstandard()
        {
            return new(&ZstandardIsCanOpen, &ZstandardOpen, &ZstandardClose, &ZstandardDecompress, null);
        }
    }

    private sealed partial class StreamInputStream : Stream
    {
        private readonly StreamInput* input;
        private readonly ulong begin;
        private readonly ulong end;
        private ulong position;

        public StreamInputStream(StreamInput* input, ulong begin, ulong end)
        {
            this.input = input;
            this.begin = begin;
            this.end = end;
            position = begin;
        }

        public override bool CanRead { get => true; }

        public override bool CanSeek { get => true; }

        public override bool CanWrite { get => false; }

        public override long Length { get => (long)(end - begin); }

        public override long Position { get => (long)(position - begin); set => position = (ulong)value + begin; }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > (int)(end - position))
            {
                count = (int)(end - position);
            }

            if (count <= 0)
            {
                return 0;
            }

            fixed (byte* pBuffer = buffer)
            {
                if (input->Read(input, position, pBuffer, pBuffer + count))
                {
                    position += (ulong)count;
                    return count;
                }
            }

            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw HutaoException.NotSupported();
        }

        public override void SetLength(long value)
        {
            throw HutaoException.NotSupported();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw HutaoException.NotSupported();
        }
    }
}