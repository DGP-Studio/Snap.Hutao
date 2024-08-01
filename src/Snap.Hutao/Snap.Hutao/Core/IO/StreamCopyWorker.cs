// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using System.Buffers;
using System.IO;

namespace Snap.Hutao.Core.IO;

internal delegate TStatus StreamCopyStatusFactory<TStatus>(long bytesReadSinceLastReport, long bytesReadSinceCopyStart);

internal sealed class StreamCopyWorker : StreamCopyWorker<StreamCopyStatus>
{
    public StreamCopyWorker(Stream source, Stream destination, long totalBytes, int bufferSize = 81920)
        : base(source, destination, (lastReport, copyStart) => new StreamCopyStatus(lastReport, copyStart, totalBytes), bufferSize)
    {
    }
}

[SuppressMessage("", "SA1402")]
internal class StreamCopyWorker<TStatus>
{
    private readonly Stream source;
    private readonly Stream destination;
    private readonly int bufferSize;
    private readonly StreamCopyStatusFactory<TStatus> statusFactory;

    public StreamCopyWorker(Stream source, Stream destination, StreamCopyStatusFactory<TStatus> statusFactory, int bufferSize = 81920)
    {
        Verify.Operation(source.CanRead, "Source Stream can't read");
        Verify.Operation(destination.CanWrite, "Destination Stream can't write");

        this.source = source;
        this.destination = destination;
        this.statusFactory = statusFactory;
        this.bufferSize = bufferSize;
    }

    public async ValueTask CopyAsync(IProgress<TStatus> progress, CancellationToken token = default)
    {
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();

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

                if (stopwatch.GetElapsedTime().TotalMilliseconds > 1000)
                {
                    progress.Report(statusFactory(bytesReadSinceLastReport, bytesReadSinceCopyStart));
                    bytesReadSinceLastReport = 0;
                    stopwatch = ValueStopwatch.StartNew();
                }
            }
            while (bytesRead > 0);
        }
    }
}