// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.Threading.RateLimiting;
using System.Buffers;
using System.IO;
using System.Net.Http;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Core.IO.Http.Sharding;

internal static class HttpShardCopyWorker
{
    [SuppressMessage("", "CA2000")]
    public static async ValueTask<IHttpShardCopyWorker<TStatus>> CreateAsync<TStatus>(HttpShardCopyWorkerOptions<TStatus> options)
    {
        await options.DetectContentLengthAsync().ConfigureAwait(false);
        return new HttpShardCopyWorker<TStatus>(options);
    }
}

[SuppressMessage("", "SA1402")]
file sealed class HttpShardCopyWorker<TStatus> : IHttpShardCopyWorker<TStatus>
{
    private readonly HttpClient httpClient;
    private readonly string sourceUrl;
    private readonly StreamCopyStatusFactory<TStatus> statusFactory;
    private readonly long contentLength;
    private readonly int bufferSize;
    private readonly SafeFileHandle destFileHandle;
    private readonly int maxDegreeOfParallelism;
    private readonly TokenBucketRateLimiter progressReportRateLimiter;

    public HttpShardCopyWorker(HttpShardCopyWorkerOptions<TStatus> options)
    {
        httpClient = options.HttpClient;
        sourceUrl = options.SourceUrl;
        statusFactory = options.StatusFactory;
        contentLength = options.ContentLength;
        bufferSize = options.BufferSize;
        destFileHandle = options.GetFileHandle();
        maxDegreeOfParallelism = options.MaxDegreeOfParallelism;

        progressReportRateLimiter = ProgressReportRateLimiter.Create(500);
    }

    [SuppressMessage("", "SH003")]
    public Task CopyAsync(IProgress<TStatus> progress, CancellationToken token = default)
    {
        PrivateShardProgress shardProgress = new(progress, statusFactory, contentLength);
        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism,
        };

        long minimumLength = Math.Max(bufferSize * maxDegreeOfParallelism, contentLength / (maxDegreeOfParallelism * 4));
        return Parallel.ForEachAsync(new AsyncHttpShards(contentLength, minimumLength), parallelOptions, (shard, token) => CopyShardAsync(shard, shardProgress, token));

        async ValueTask CopyShardAsync(IHttpShard shard, IProgress<PrivateShardStatus> progress, CancellationToken token)
        {
            HttpRequestMessage request = new(HttpMethod.Get, sourceUrl)
            {
                Headers = { Range = new(shard.Start, shard.End), },
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
                            int bytesReadSinceLastReport = 0;
                            do
                            {
                                using (await shard.ReaderWriterLock.ReaderLockAsync().ConfigureAwait(false))
                                {
                                    if (shard.BytesRead >= shard.End - shard.Start)
                                    {
                                        break;
                                    }

                                    bool report = true;
                                    try
                                    {
                                        int bytesRead = await stream.ReadAsync(buffer, token).ConfigureAwait(false);
                                        if (bytesRead <= 0)
                                        {
                                            break;
                                        }

                                        await RandomAccess.WriteAsync(destFileHandle, buffer[..bytesRead], shard.Start + shard.BytesRead, token).ConfigureAwait(false);

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
    }

    public void Dispose()
    {
        destFileHandle.Dispose();
        progressReportRateLimiter.Dispose();
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