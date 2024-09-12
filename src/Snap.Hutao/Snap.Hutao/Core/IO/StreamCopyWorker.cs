﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Buffers;
using System.IO;
using System.Threading.RateLimiting;
using Snap.Hutao.Core.Threading.RateLimiting;

namespace Snap.Hutao.Core.IO;

internal delegate TStatus StreamCopyStatusFactory<TStatus>(long bytesReadSinceLastReport, long bytesReadSinceCopyStart);

internal sealed partial class StreamCopyWorker : StreamCopyWorker<StreamCopyStatus>
{
    public StreamCopyWorker(Stream source, Stream destination, long totalBytes, int bufferSize = 81920)
        : base(source, destination, (lastReport, copyStart) => new StreamCopyStatus(lastReport, copyStart, totalBytes), bufferSize)
    {
    }
}

[SuppressMessage("", "SA1402")]
internal partial class StreamCopyWorker<TStatus> : IDisposable
{
#pragma warning disable CA2213
    private readonly Stream source;
    private readonly Stream destination;
#pragma warning restore CA2213

    private readonly int bufferSize;
    private readonly StreamCopyStatusFactory<TStatus> statusFactory;
    private readonly TokenBucketRateLimiter progressReportRateLimiter;

    public StreamCopyWorker(Stream source, Stream destination, StreamCopyStatusFactory<TStatus> statusFactory, int bufferSize = 81920)
    {
        Verify.Operation(source.CanRead, "Source Stream can't read");
        Verify.Operation(destination.CanWrite, "Destination Stream can't write");

        this.source = source;
        this.destination = destination;
        this.statusFactory = statusFactory;
        this.bufferSize = bufferSize;

        progressReportRateLimiter = ProgressReportRateLimiter.Create(1000);
    }

    public async ValueTask CopyAsync(IProgress<TStatus> progress, CancellationToken token = default)
    {
        long bytesReadSinceCopyStart = 0;
        long bytesReadSinceLastReport = 0;

        int bytesRead;

        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(bufferSize))
        {
            Memory<byte> buffer = memoryOwner.Memory;

            do
            {
                bytesRead = await source.ReadAsync(buffer, token).ConfigureAwait(false);
                if (bytesRead is 0)
                {
                    progress.Report(statusFactory(bytesReadSinceLastReport, bytesReadSinceCopyStart));
                    break;
                }

                await destination.WriteAsync(buffer[..bytesRead], token).ConfigureAwait(false);

                bytesReadSinceCopyStart += bytesRead;
                bytesReadSinceLastReport += bytesRead;

                if (progressReportRateLimiter.AttemptAcquire().IsAcquired)
                {
                    progress.Report(statusFactory(bytesReadSinceLastReport, bytesReadSinceCopyStart));
                    bytesReadSinceLastReport = 0;
                }
            }
            while (bytesRead > 0);
        }
    }

    public async ValueTask CopyAsync(TokenBucketRateLimiter rateLimiter, IProgress<TStatus> progress, CancellationToken token = default)
    {
        long bytesReadSinceCopyStart = 0;
        long bytesReadSinceLastReport = 0;

        int bytesRead;

        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(bufferSize))
        {
            Memory<byte> buffer = memoryOwner.Memory;

            do
            {
                if (!rateLimiter.TryAcquire(buffer.Length, out int bytesToRead, out TimeSpan retryAfter))
                {
                    await Task.Delay(retryAfter, token).ConfigureAwait(false);
                    continue;
                }

                bytesRead = await source.ReadAsync(buffer[..bytesToRead], token).ConfigureAwait(false);
                if (bytesRead is 0)
                {
                    progress.Report(statusFactory(bytesReadSinceLastReport, bytesReadSinceCopyStart));
                    break;
                }

                rateLimiter.Replenish(bytesToRead - bytesRead);
                await destination.WriteAsync(buffer[..bytesRead], token).ConfigureAwait(false);

                bytesReadSinceCopyStart += bytesRead;
                bytesReadSinceLastReport += bytesRead;

                if (progressReportRateLimiter.AttemptAcquire().IsAcquired)
                {
                    progress.Report(statusFactory(bytesReadSinceLastReport, bytesReadSinceCopyStart));
                    bytesReadSinceLastReport = 0;
                }
            }
            while (true);
        }
    }

    public void Dispose()
    {
        progressReportRateLimiter.Dispose();
    }
}