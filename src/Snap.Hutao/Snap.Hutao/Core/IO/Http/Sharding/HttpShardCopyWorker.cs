// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Threading.RateLimiting;
using System.Buffers;
using System.IO;
using System.Net.Http;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Core.IO.Http.Sharding;

internal static class HttpShardCopyWorker
{
    public static async ValueTask<IHttpShardCopyWorker<TStatus>> CreateAsync<TStatus>(HttpShardCopyWorkerOptions<TStatus> options)
    {
        await options.DetectContentLengthAsync().ConfigureAwait(false);
        return new HttpShardCopyWorker<TStatus>(options);
    }
}

[SuppressMessage("", "SA1402")]
internal sealed partial class HttpShardCopyWorker<TStatus> : IHttpShardCopyWorker<TStatus>
{
    private readonly HttpShardCopyWorkerOptions<TStatus> options;
    private readonly TokenBucketRateLimiter progressReportRateLimiter;

    public HttpShardCopyWorker(HttpShardCopyWorkerOptions<TStatus> options)
    {
        options.MakeReadOnly();
        this.options = options;

        progressReportRateLimiter = ProgressReportRateLimiter.Create(500);
    }

    [SuppressMessage("", "SH003")]
    public Task CopyAsync(IProgress<TStatus> progress, CancellationToken token = default)
    {
        PrivateShardProgress shardProgress = new(progress, options.StatusFactory, options.ContentLength);
        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = options.MaxDegreeOfParallelism,
        };

        long minimumLength = Math.Max(options.BufferSize * options.MaxDegreeOfParallelism, options.ContentLength / (options.MaxDegreeOfParallelism * 4));
        return Parallel.ForEachAsync(new AsyncHttpShards(options.ContentLength, minimumLength), parallelOptions, (shard, token) => CopyShardAsync(shard, shardProgress, token));
    }

    public void Dispose()
    {
        options.DestinationFileHandle.Dispose();
        progressReportRateLimiter.Dispose();
    }

    private async ValueTask CopyShardAsync(IHttpShard shard, IProgress<PrivateShardStatus> progress, CancellationToken token)
    {
        HttpRequestMessage request = new(HttpMethod.Get, options.SourceUrl)
        {
            Headers = { Range = new(shard.Start, shard.End), },
        };

        using (request)
        {
            using (HttpResponseMessage response = await options.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(true))
            {
                response.EnsureSuccessStatusCode();
                using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(options.BufferSize))
                {
                    Memory<byte> buffer = memoryOwner.Memory;
                    using (Stream stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(true))
                    {
                        int bytesReadSinceLastReport = 0;
                        do
                        {
                            using (await shard.ReaderWriterLock.ReaderLockAsync().ConfigureAwait(true))
                            {
                                if (shard.BytesRead >= shard.End - shard.Start)
                                {
                                    break;
                                }

                                bool report = true;
                                try
                                {
                                    int bytesRead = await stream.ReadAsync(buffer, token).ConfigureAwait(true);
                                    if (bytesRead <= 0)
                                    {
                                        break;
                                    }

                                    await RandomAccess.WriteAsync(options.DestinationFileHandle, buffer[..bytesRead], shard.Start + shard.BytesRead, token).ConfigureAwait(true);

                                    shard.BytesRead += bytesRead;
                                    bytesReadSinceLastReport += bytesRead;
                                    if (!progressReportRateLimiter.AttemptAcquire().IsAcquired)
                                    {
                                        report = false;
                                    }
                                }
                                finally
                                {
                                    if (report)
                                    {
                                        progress.Report(new(bytesReadSinceLastReport));
                                        bytesReadSinceLastReport = 0;
                                    }
                                }
                            }
                        }
                        while (true);
                    }
                }
            }
        }
    }

    private sealed class PrivateShardStatus
    {
        public PrivateShardStatus(int bytesRead)
        {
            BytesRead = bytesRead;
        }

        public int BytesRead { get; }
    }

    private sealed class PrivateShardProgress : IProgress<PrivateShardStatus>
    {
        private readonly IProgress<TStatus> workerProgress;
        private readonly StreamCopyStatusFactory<TStatus> statusFactory;
        private readonly long contentLength;
        private readonly TokenBucketRateLimiter progressReportRateLimiter = ProgressReportRateLimiter.Create(1000);
        private long totalBytesRead;

        public PrivateShardProgress(IProgress<TStatus> workerProgress, StreamCopyStatusFactory<TStatus> statusFactory, long contentLength)
        {
            this.workerProgress = workerProgress;
            this.statusFactory = statusFactory;
            this.contentLength = contentLength;
        }

        public void Report(PrivateShardStatus value)
        {
            if (Interlocked.Add(ref totalBytesRead, value.BytesRead) == contentLength || progressReportRateLimiter.AttemptAcquire().IsAcquired)
            {
                workerProgress.Report(statusFactory(totalBytesRead, contentLength));
            }
        }
    }
}