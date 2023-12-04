// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Web.Request.Builder;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Core.IO.Http.Sharding;

internal sealed class HttpShardCopyWorker<TStatus> : IDisposable
{
    private const int ShardSize = 4 * 1024 * 1024;

    private readonly HttpClient httpClient;
    private readonly string sourceUrl;
    private readonly Func<long, TStatus> statusFactory;
    private readonly int bufferSize;

    private readonly SafeFileHandle destFileHandle;
    private readonly List<Shard> shards;

    private HttpShardCopyWorker(HttpClient httpClient, string sourceUrl, string destFilePath, long contentLength, Func<long, TStatus> statusFactory, int bufferSize)
    {
        this.httpClient = httpClient;
        this.sourceUrl = sourceUrl;
        this.statusFactory = statusFactory;
        this.bufferSize = bufferSize;

        destFileHandle = File.OpenHandle(
            destFilePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            FileOptions.RandomAccess | FileOptions.Asynchronous,
            contentLength);

        shards = CalculateShards(contentLength);
    }

    public static async ValueTask<HttpShardCopyWorker<TStatus>> CreateAsync(HttpClient httpClient, string sourceUrl, string destFilePath, Func<long, TStatus> statusFactory, int bufferSize = 81920)
    {
        HttpResponseMessage response = await httpClient.HeadAsync(sourceUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        long contentLength = response.Content.Headers.ContentLength ?? 0;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(contentLength);

        return new(httpClient, sourceUrl, destFilePath, contentLength, statusFactory, bufferSize);
    }

    [SuppressMessage("", "SH003")]
    public Task CopyAsync(IProgress<TStatus> progress, CancellationToken token = default)
    {
        ShardProgress shardProgress = new(progress, statusFactory);
        return Parallel.ForEachAsync(shards, token, (shard, token) => CopyShardAsync(shard, shardProgress, token));
    }

    public void Dispose()
    {
        destFileHandle.Dispose();
    }

    private static List<Shard> CalculateShards(long contentLength)
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

    private async ValueTask CopyShardAsync(Shard shard, IProgress<ShardStatus> progress, CancellationToken token)
    {
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();
        HttpRequestMessage request = new(HttpMethod.Get, sourceUrl)
        {
            Headers =
            {
                Range = new(shard.StartOffset, shard.EndOffset),
            },
        };

        using (request)
        {
            using (HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                Memory<byte> buffer = new byte[bufferSize];
                using (Stream stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
                {
                    int totalBytesRead = 0;
                    do
                    {
                        int bytesRead = await stream.ReadAsync(buffer, token).ConfigureAwait(false);
                        if (bytesRead <= 0)
                        {
                            progress.Report(new(totalBytesRead));
                            break;
                        }

                        await RandomAccess.WriteAsync(destFileHandle, buffer[..bytesRead], shard.StartOffset + totalBytesRead, token).ConfigureAwait(false);

                        totalBytesRead += bytesRead;
                        if (stopwatch.GetElapsedTime().TotalMilliseconds > 500)
                        {
                            progress.Report(new(totalBytesRead));
                            stopwatch = ValueStopwatch.StartNew();
                        }
                    }
                    while (true);
                }
            }
        }
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

        public int BytesRead { get; set; }
    }

    private sealed class ShardProgress : IProgress<ShardStatus>
    {
        private readonly IProgress<TStatus> workerProgress;
        private readonly Func<long, TStatus> statusFactory;
        private readonly object syncRoot = new();
        private ValueStopwatch stopwatch = ValueStopwatch.StartNew();
        private long totalBytesRead;

        public ShardProgress(IProgress<TStatus> workerProgress, Func<long, TStatus> statusFactory)
        {
            this.workerProgress = workerProgress;
            this.statusFactory = statusFactory;
        }

        public void Report(ShardStatus value)
        {
            Interlocked.Add(ref totalBytesRead, value.BytesRead);
            if (stopwatch.GetElapsedTime().TotalMilliseconds > 500)
            {
                lock (syncRoot)
                {
                    if (stopwatch.GetElapsedTime().TotalMilliseconds > 500)
                    {
                        workerProgress.Report(statusFactory(totalBytesRead));
                        stopwatch = ValueStopwatch.StartNew();
                    }
                }
            }
        }
    }
}