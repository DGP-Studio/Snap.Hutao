// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.ZStandard;
using System.Buffers;
using System.IO;
using static Snap.ZStandard.Methods;

namespace Snap.Hutao.Core.IO.Compression.Zstandard;

internal sealed partial class ZstandardDecompressionStream : Stream
{
    [SuppressMessage("", "CA2213")]
    private readonly Stream inputStream;
    private readonly IMemoryOwner<byte> inputBufferMemoryOwner;
    private readonly Memory<byte> inputBuffer;

    private unsafe ZSTD_DCtx_s* decompressStreamContext;
    private nuint pos;
    private nuint size;

    public unsafe ZstandardDecompressionStream(Stream inputStream, int bufferSize = 0)
    {
        ArgumentNullException.ThrowIfNull(inputStream);

        if (!inputStream.CanRead)
        {
            throw new ArgumentException("Stream is not readable", nameof(inputStream));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(bufferSize);

        this.inputStream = inputStream;

        ZstandardException.ThrowIfNull(decompressStreamContext = ZSTD_createDStream(), "Failed to create decompression stream");
        ZstandardException.ThrowIfError(ZSTD_DCtx_reset(decompressStreamContext, ZSTD_ResetDirective.ZSTD_reset_session_only));

        int inputBufferSize;

        if (bufferSize > 0)
        {
            inputBufferSize = bufferSize;
        }
        else
        {
            nuint size = ZSTD_DStreamInSize();
            ZstandardException.ThrowIfError(size);
            inputBufferSize = (int)size;
        }

        inputBufferMemoryOwner = MemoryPool<byte>.Shared.Rent(inputBufferSize);
        inputBuffer = inputBufferMemoryOwner.Memory[..inputBufferSize];
        pos = size = (nuint)inputBufferSize;
    }

    ~ZstandardDecompressionStream()
    {
        Dispose(false);
    }

    public override bool CanRead { get => true; }

    public override bool CanSeek { get => false; }

    public override bool CanWrite { get => false; }

    public override long Length { get => throw new NotSupportedException(); }

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
        throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public override unsafe int Read(Span<byte> buffer)
    {
        ObjectDisposedException.ThrowIf(decompressStreamContext is null, this);
        return ReadInternal(buffer);
    }

    public override unsafe int Read(byte[] buffer, int offset, int count)
    {
        CheckParamsValid(buffer, offset, count);
        ObjectDisposedException.ThrowIf(decompressStreamContext is null, this);
        return ReadInternal(new Span<byte>(buffer, offset, count));
    }

    public override unsafe ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(decompressStreamContext is null, this);
        return ReadInternalAsync(buffer, token);
    }

    public override unsafe Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token = default)
    {
        CheckParamsValid(buffer, offset, count);
        ObjectDisposedException.ThrowIf(decompressStreamContext is null, this);
        return ReadInternalAsync(new Memory<byte>(buffer, offset, count), token).AsTask();
    }

    protected override unsafe void Dispose(bool disposing)
    {
        if (decompressStreamContext is null)
        {
            return;
        }

        ZSTD_freeDStream(decompressStreamContext);

        inputBufferMemoryOwner.Dispose();

        decompressStreamContext = default;

        base.Dispose(disposing);
    }

    private static void CheckParamsValid(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if (count > buffer.Length - offset)
        {
            throw new ArgumentException("The sum of offset and count is greater than the buffer length");
        }
    }

    private int ReadInternal(Span<byte> buffer)
    {
        ZSTD_inBuffer_s input = default;
        input.size = size;
        input.pos = pos;
        ZSTD_outBuffer_s output = default;
        output.size = (nuint)buffer.Length;
        output.pos = 0;

        while (output.size > output.pos)
        {
            if (input.size <= input.pos)
            {
                int bytesRead = inputStream.Read(inputBuffer.Span);
                if (bytesRead <= 0)
                {
                    break;
                }

                input.size = (nuint)bytesRead;
                input.pos = 0;
            }

            Decompress(buffer, ref output, ref input);
        }

        pos = input.pos;
        size = input.size;

        return (int)output.pos;
    }

    private async ValueTask<int> ReadInternalAsync(Memory<byte> buffer, CancellationToken token)
    {
        ZSTD_inBuffer_s input = default;
        input.size = size;
        input.pos = pos;
        ZSTD_outBuffer_s output = default;
        output.size = (nuint)buffer.Length;
        output.pos = 0;

        while (output.size > output.pos)
        {
            if (input.size <= input.pos)
            {
                int bytesRead = await inputStream.ReadAsync(inputBuffer, token).ConfigureAwait(false);
                if (bytesRead <= 0)
                {
                    break;
                }

                input.size = (nuint)bytesRead;
                input.pos = 0;
            }

            Decompress(buffer.Span, ref output, ref input);
        }

        pos = input.pos;
        size = input.size;

        return (int)output.pos;
    }

    private unsafe void Decompress(Span<byte> outputBuffer, ref ZSTD_outBuffer_s output, ref ZSTD_inBuffer_s input)
    {
        fixed (void* pOutputBuffer = outputBuffer)
        {
            fixed (void* pInputBuffer = inputBuffer.Span)
            {
                output.dst = pOutputBuffer;
                input.src = pInputBuffer;

                fixed (ZSTD_outBuffer_s* pOutput = &output)
                {
                    fixed (ZSTD_inBuffer_s* pInput = &input)
                    {
                        ZstandardException.ThrowIfError(ZSTD_decompressStream(decompressStreamContext, pOutput, pInput));
                    }
                }
            }
        }
    }
}