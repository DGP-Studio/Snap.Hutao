// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.ZStandard;
using System.Buffers;
using System.IO;
using static Snap.ZStandard.Zstandard;

namespace Snap.Hutao.Core.IO.Compression.Zstandard;

// See https://github.com/skbkontur/ZstdNet
// ReSharper disable LocalizableElement
internal sealed partial class ZstandardDecompressStream : Stream
{
    [SuppressMessage("", "CA2213")]
    private readonly Stream inputStream;
    private readonly IMemoryOwner<byte> inputBufferMemoryOwner;
    private readonly Memory<byte> inputBuffer;

    private unsafe ZSTD_DCtx_s* decompressStreamContext;
    private nuint position;
    private nuint size;

    public unsafe ZstandardDecompressStream(Stream inputStream, int bufferSize = 0)
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
            nuint autoSize = ZSTD_DStreamInSize();
            ZstandardException.ThrowIfError(autoSize);
            inputBufferSize = (int)autoSize;
        }

        inputBufferMemoryOwner = MemoryPool<byte>.Shared.RentExactly(inputBufferSize);
        inputBuffer = inputBufferMemoryOwner.Memory;
        position = size = (nuint)inputBufferSize;
    }

    ~ZstandardDecompressStream()
    {
        Dispose(false);
    }

    public override bool CanRead { get => true; }

    public override bool CanSeek { get => false; }

    public override bool CanWrite { get => false; }

    public override long Length { get => throw HutaoException.NotSupported(); }

    public override long Position
    {
        get => throw HutaoException.NotSupported();
        set => throw HutaoException.NotSupported();
    }

    public override void Flush()
    {
        throw HutaoException.NotSupported();
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

    public override unsafe int Read(Span<byte> buffer)
    {
        ObjectDisposedException.ThrowIf(decompressStreamContext is null, this);
        return ReadInternal(buffer);
    }

    public override unsafe int Read(byte[] buffer, int offset, int count)
    {
        CheckParamsValid(buffer, offset, count);
        ObjectDisposedException.ThrowIf(decompressStreamContext is null, this);
        return ReadInternal(buffer.AsSpan(offset, count));
    }

    public override unsafe ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(decompressStreamContext is null, this);
        return ReadInternalAsync(buffer, token);
    }

    public override unsafe Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
    {
        CheckParamsValid(buffer, offset, count);
        ObjectDisposedException.ThrowIf(decompressStreamContext is null, this);
        return ReadInternalAsync(buffer.AsMemory(offset, count), token).AsTask();
    }

    protected override unsafe void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (decompressStreamContext is null)
        {
            return;
        }

        ZSTD_freeDStream(decompressStreamContext);

        inputBufferMemoryOwner.Dispose();

        decompressStreamContext = default;
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
        input.pos = position;
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

        position = input.pos;
        size = input.size;

        return (int)output.pos;
    }

    private async ValueTask<int> ReadInternalAsync(Memory<byte> buffer, CancellationToken token)
    {
        ZSTD_inBuffer_s input = default;
        input.size = size;
        input.pos = position;
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

        position = input.pos;
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