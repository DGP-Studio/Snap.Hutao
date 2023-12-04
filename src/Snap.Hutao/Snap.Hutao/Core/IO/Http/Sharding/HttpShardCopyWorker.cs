// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Web.Request.Builder;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Core.IO.Http.Sharding;

internal sealed class HttpShardCopyWorker : IDisposable
{
    private const int ShardSize = 4 * 1024 * 1024;

    private readonly HttpClient httpClient;
    private readonly string sourceUrl;
    private readonly int bufferSize;
    private readonly SafeFileHandle destFileHandle;
    private readonly List<Shard> shards;

    private HttpShardCopyWorker(HttpClient httpClient, string sourceUrl, string destFilePath, long contentLength, int bufferSize)
    {
        this.httpClient = httpClient;
        this.sourceUrl = sourceUrl;
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

    public static async ValueTask<HttpShardCopyWorker> CreateAsync(HttpClient httpClient, string sourceUrl, string destFilePath, int bufferSize = 81920)
    {
        HttpResponseMessage response = await httpClient.HeadAsync(sourceUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        long contentLength = response.Content.Headers.ContentLength ?? 0;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(contentLength);

        return new(httpClient, sourceUrl, destFilePath, contentLength, bufferSize);
    }

    [SuppressMessage("", "SH003")]
    public Task CopyAsync(CancellationToken cancellationToken = default)
    {
        return Parallel.ForEachAsync(shards, cancellationToken, CopyShardAsync);
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

    private async ValueTask CopyShardAsync(Shard shard, CancellationToken token)
    {
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
                    int streamOffset = 0;
                    do
                    {
                        int bytesRead = await stream.ReadAsync(buffer, token).ConfigureAwait(false);
                        if (bytesRead <= 0)
                        {
                            break;
                        }

                        await RandomAccess.WriteAsync(destFileHandle, buffer[..bytesRead], shard.StartOffset + streamOffset, token).ConfigureAwait(false);
                        streamOffset += bytesRead;
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
}