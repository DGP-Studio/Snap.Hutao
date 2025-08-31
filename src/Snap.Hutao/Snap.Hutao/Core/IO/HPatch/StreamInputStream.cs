// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.IO;

namespace Snap.Hutao.Core.IO.HPatch;

internal sealed unsafe partial class StreamInputStream : Stream
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