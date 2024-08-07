// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using System.Buffers;
using System.IO;
using System.Net.Http;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Core.IO.Http.Sharding;

// TODO: refactor to use tree structure to calculate shards
internal sealed class HttpShardCopyWorker<TStatus> : IDisposable
{
    private const int ShardSize = 4 * 1024 * 1024;

    private readonly HttpClient httpClient;
    private readonly string sourceUrl;
    private readonly StreamCopyStatusFactory<TStatus> statusFactory;
    private readonly long contentLength;
    private readonly int bufferSize;
    private readonly SafeFileHandle destFileHandle;
    private readonly int maxDegreeOfParallelism;
    private readonly List<Shard> shards;
    private readonly TokenBucketRateLimiter progressReportRateLimiter;

    private HttpShardCopyWorker(HttpShardCopyWorkerOptions<TStatus> options)
    {
        httpClient = options.HttpClient;
        sourceUrl = options.SourceUrl;
        statusFactory = options.StatusFactory;
        contentLength = options.ContentLength;
        bufferSize = options.BufferSize;
        destFileHandle = options.GetFileHandle();
        maxDegreeOfParallelism = options.MaxDegreeOfParallelism;
        shards = CalculateShards(contentLength);

        progressReportRateLimiter = ProgressReportRateLimiter.Create(500);

        static List<Shard> CalculateShards(long contentLength)
        {
            List<Shard> shards = [];
            long currentOffset = 0;

            while (currentOffset < contentLength)
            {
                long end = Math.Min(currentOffset + ShardSize, contentLength) - 1;
                shards.Add(new Shard(currentOffset, end));
                currentOffset = end + 1;
            }

            return shards;
        }
    }

    public static async ValueTask<HttpShardCopyWorker<TStatus>> CreateAsync(HttpShardCopyWorkerOptions<TStatus> options)
    {
        await options.DetectContentLengthAsync().ConfigureAwait(false);
        return new(options);
    }

    [SuppressMessage("", "SH003")]
    public Task CopyAsync(IProgress<TStatus> progress, CancellationToken token = default)
    {
        ShardProgress shardProgress = new(progress, statusFactory, contentLength);
        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism,
        };
        return Parallel.ForEachAsync(shards, parallelOptions, (shard, token) => CopyShardAsync(shard, shardProgress, token));

        async ValueTask CopyShardAsync(Shard shard, IProgress<ShardStatus> progress, CancellationToken token)
        {
            HttpRequestMessage request = new(HttpMethod.Get, sourceUrl)
            {
                Headers = { Range = new(shard.StartOffset, shard.EndOffset), },
            };

            using (request)
            {
                using (HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(bufferSize))
                    {
                        Memory<byte> buffer = memoryOwner.Memory;
                        using (Stream stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
                        {
                            int totalBytesRead = 0;
                            int bytesReadSinceLastReport = 0;
                            do
                            {
                                int bytesRead = await stream.ReadAsync(buffer, token).ConfigureAwait(false);
                                if (bytesRead <= 0)
                                {
                                    progress.Report(new(bytesReadSinceLastReport));
                                    bytesReadSinceLastReport = 0;
                                    break;
                                }

                                await RandomAccess.WriteAsync(destFileHandle, buffer[..bytesRead], shard.StartOffset + totalBytesRead, token).ConfigureAwait(false);

                                totalBytesRead += bytesRead;
                                bytesReadSinceLastReport += bytesRead;
                                if (progressReportRateLimiter.AttemptAcquire().IsAcquired)
                                {
                                    progress.Report(new(bytesReadSinceLastReport));
                                    bytesReadSinceLastReport = 0;
                                }
                            }
                            while (true);
                        }
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        destFileHandle.Dispose();
        progressReportRateLimiter.Dispose();
    }

    private sealed class Shard
    {
        public Shard(long startOffset, long endOffset)
        {
            StartOffset = startOffset;
            EndOffset = endOffset;
        }

        public long StartOffset { get; }

        public long EndOffset { get; }
    }

    private sealed class ShardStatus
    {
        public ShardStatus(int bytesRead)
        {
            BytesRead = bytesRead;
        }

        public int BytesRead { get; }
    }

    private sealed class ShardProgress : IProgress<ShardStatus>
    {
        private readonly IProgress<TStatus> workerProgress;
        private readonly Func<long, long, TStatus> statusFactory;
        private readonly long contentLength;
        private readonly TokenBucketRateLimiter progressReportRateLimiter = ProgressReportRateLimiter.Create(1000);
        private long totalBytesRead;

        public ShardProgress(IProgress<TStatus> workerProgress, Func<long, long, TStatus> statusFactory, long contentLength)
        {
            this.workerProgress = workerProgress;
            this.statusFactory = statusFactory;
            this.contentLength = contentLength;
        }

        public void Report(ShardStatus value)
        {
            if (Interlocked.Add(ref totalBytesRead, value.BytesRead) == contentLength || progressReportRateLimiter.AttemptAcquire().IsAcquired)
            {
                workerProgress.Report(statusFactory(totalBytesRead, contentLength));
            }
        }
    }
}