// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Web.Request.Builder;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Core.IO.Http.Sharding;

internal sealed class HttpShardCopyWorkerOptions<TStatus>
{
    private readonly LazySlim<SafeFileHandle> lazyDestinationFileHandle;

    private bool isReadOnly;

    public HttpShardCopyWorkerOptions()
    {
        lazyDestinationFileHandle = new(GetFileHandle);
    }

    public required HttpClient HttpClient { get; init; }

    public required string SourceUrl { get; init; }

    public required string DestinationFilePath { get; init; }

    public required StreamCopyStatusFactory<TStatus> StatusFactory { get; init; }

    public int BufferSize { get; init; } = 80 * 1024;

    public int MaxDegreeOfParallelism { get; init; } = Environment.ProcessorCount;

    public SafeFileHandle DestinationFileHandle { get => lazyDestinationFileHandle.Value; }

    public long ContentLength
    {
        get;
        private set
        {
            Verify.Operation(!isReadOnly, "The options is read-only.");
            field = value;
        }
    }

    public async ValueTask DetectContentLengthAsync()
    {
        if (ContentLength > 0)
        {
            return;
        }

        HttpResponseMessage response = await HttpClient.HeadAsync(SourceUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        ContentLength = response.Content.Headers.ContentLength ?? 0;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ContentLength);
    }

    public void MakeReadOnly()
    {
        isReadOnly = true;
    }

    private SafeFileHandle GetFileHandle()
    {
        return File.OpenHandle(DestinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, FileOptions.RandomAccess | FileOptions.Asynchronous, ContentLength);
    }
}