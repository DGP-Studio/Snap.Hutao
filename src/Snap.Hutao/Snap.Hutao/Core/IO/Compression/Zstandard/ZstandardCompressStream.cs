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
internal sealed partial class ZstandardCompressStream : Stream
{
    [SuppressMessage("", "CA2213")]
    private readonly Stream outputStream;
    private readonly IMemoryOwner<byte> outputBufferMemoryOwner;
    private readonly Memory<byte> outputBuffer;
    private readonly nuint size;

    private unsafe ZSTD_CCtx_s* compressStreamContext;
    private nuint position;

    public unsafe ZstandardCompressStream(Stream outputStream, int bufferSize = 0)
    {
        ArgumentNullException.ThrowIfNull(outputStream);

        if (!outputStream.CanWrite)
        {
            throw new ArgumentException("Stream is not writable", nameof(outputStream));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(bufferSize);

        this.outputStream = outputStream;

        ZstandardException.ThrowIfNull(compressStreamContext = ZSTD_createCStream(), "Failed to create compression stream");
        ZstandardException.ThrowIfError(ZSTD_CCtx_reset(compressStreamContext, ZSTD_ResetDirective.ZSTD_reset_session_only));

        int outputBufferSize;
        if (bufferSize > 0)
        {
            outputBufferSize = bufferSize;
        }
        else
        {
            nuint autoSize = ZSTD_CStreamOutSize();
            ZstandardException.ThrowIfError(autoSize);
            outputBufferSize = (int)autoSize;
        }

        outputBufferMemoryOwner = MemoryPool<byte>.Shared.RentExactly(outputBufferSize);
        outputBuffer = outputBufferMemoryOwner.Memory;
        size = (nuint)outputBufferSize;
    }

    ~ZstandardCompressStream()
    {
        Dispose(false);
    }

    public override bool CanRead { get => false; }

    public override bool CanSeek { get => false; }

    public override bool CanWrite { get => true; }

    public override long Length { get => throw HutaoException.NotSupported(); }

    public override long Position
    {
        get => throw HutaoException.NotSupported();
        set => throw HutaoException.NotSupported();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw HutaoException.NotSupported();
    }

    public override void SetLength(long value)
    {
        throw HutaoException.NotSupported();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw HutaoException.NotSupported();
    }

    public override unsafe void Flush()
    {
        ObjectDisposedException.ThrowIf(compressStreamContext is null, this);
        FlushInternal(ZSTD_EndDirective.ZSTD_e_flush);
    }

    public override unsafe Task FlushAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(compressStreamContext is null, this);
        return FlushInternalAsync(ZSTD_EndDirective.ZSTD_e_flush, cancellationToken).AsTask();
    }

    public override unsafe void Write(ReadOnlySpan<byte> buffer)
    {
        ObjectDisposedException.ThrowIf(compressStreamContext is null, this);
        WriteInternal(buffer);
    }

    public override unsafe void Write(byte[] buffer, int offset, int count)
    {
        CheckParamsValid(buffer, offset, count);
        ObjectDisposedException.ThrowIf(compressStreamContext is null, this);
        WriteInternal(new Span<byte>(buffer, offset, count));
    }

    public override unsafe ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(compressStreamContext is null, this);
        return WriteInternalAsync(buffer, cancellationToken);
    }

    public override unsafe Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        CheckParamsValid(buffer, offset, count);
        ObjectDisposedException.ThrowIf(compressStreamContext is null, this);
        return WriteInternalAsync(new(buffer, offset, count), cancellationToken).AsTask();
    }

    protected override unsafe void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (compressStreamContext is null)
        {
            return;
        }

        try
        {
            if (!disposing)
            {
                return;
            }

            FlushInternal(ZSTD_EndDirective.ZSTD_e_end);
        }
        finally
        {
            ZSTD_freeCStream(compressStreamContext);

            outputBufferMemoryOwner.Dispose();
            compressStreamContext = default;
        }
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

    private void WriteInternal(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length == 0)
        {
            return;
        }

        ZSTD_inBuffer_s input = default;
        input.size = (nuint)buffer.Length;
        input.pos = 0;
        ZSTD_outBuffer_s output = default;
        output.size = size;
        output.pos = position;

        Span<byte> outputSpan = outputBuffer.Span;

        do
        {
            if (output.size <= output.pos)
            {
                outputStream.Write(outputSpan[..(int)output.pos]);
                output.pos = 0;
            }

            Compress(buffer, ref output, ref input, ZSTD_EndDirective.ZSTD_e_continue);
        }
        while (input.size > input.pos);

        position = output.pos;
    }

    private async ValueTask WriteInternalAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        if (buffer.Length == 0)
        {
            return;
        }

        ZSTD_inBuffer_s input = default;
        input.size = (nuint)buffer.Length;
        input.pos = 0;
        ZSTD_outBuffer_s output = default;
        output.size = size;
        output.pos = position;

        do
        {
            if (output.size <= output.pos)
            {
                await outputStream.WriteAsync(outputBuffer[..(int)output.pos], cancellationToken).ConfigureAwait(false);
                output.pos = 0;
            }

            Compress(buffer.Span, ref output, ref input, ZSTD_EndDirective.ZSTD_e_continue);
        }
        while (input.size > input.pos);

        position = output.pos;
    }

    private void FlushInternal(ZSTD_EndDirective directive)
    {
        ZSTD_inBuffer_s input = default;
        input.size = 0;
        input.pos = 0;
        ZSTD_outBuffer_s output = default;
        output.size = size;
        output.pos = position;

        Span<byte> outputSpan = outputBuffer.Span;

        do
        {
            if (output.size <= output.pos)
            {
                outputStream.Write(outputSpan[..(int)output.pos]);
                output.pos = UIntPtr.Zero;
            }
        }
        while (Compress([], ref output, ref input, directive) is not 0);

        if (output.pos is not 0)
        {
            outputStream.Write(outputSpan[..(int)output.pos]);
        }

        position = 0;
    }

    private async ValueTask FlushInternalAsync(ZSTD_EndDirective directive, CancellationToken cancellationToken)
    {
        ZSTD_inBuffer_s input = default;
        input.size = 0;
        input.pos = 0;
        ZSTD_outBuffer_s output = default;
        output.size = size;
        output.pos = position;

        do
        {
            if (output.size <= output.pos)
            {
                await outputStream.WriteAsync(outputBuffer[..(int)output.pos], cancellationToken).ConfigureAwait(false);
                output.pos = 0;
            }
        }
        while (Compress([], ref output, ref input, directive) is not 0);

        if (output.pos is not 0)
        {
            await outputStream.WriteAsync(outputBuffer[..(int)output.pos], cancellationToken).ConfigureAwait(false);
        }

        position = 0;
    }

    private unsafe nuint Compress(ReadOnlySpan<byte> inputBuffer, ref ZSTD_outBuffer_s output, ref ZSTD_inBuffer_s input, ZSTD_EndDirective directive)
    {
        fixed (void* pInputBuffer = inputBuffer)
        {
            fixed (void* pOutputBuffer = outputBuffer.Span)
            {
                input.src = pInputBuffer;
                output.dst = pOutputBuffer;

                fixed (ZSTD_inBuffer_s* pInput = &input)
                {
                    fixed (ZSTD_outBuffer_s* pOutput = &output)
                    {
                        nuint result = ZSTD_compressStream2(compressStreamContext, pOutput, pInput, directive);
                        ZstandardException.ThrowIfError(result);
                        return result;
                    }
                }
            }
        }
    }
}