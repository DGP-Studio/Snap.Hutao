﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Net.Http;
using Snap.Hutao.Web.Request.Builder;

namespace Snap.Hutao.Core.IO.Http.Sharding;

internal sealed class HttpShardCopyWorkerOptions<TStatus>
{
    public HttpClient HttpClient { get; set; } = default!;

    public string SourceUrl { get; set; } = default!;

    public string DestinationFilePath { get; set; } = default!;

    public long ContentLength { get; private set; }

    public Func<long, long, TStatus> StatusFactory { get; set; } = default!;

    public int BufferSize { get; set; } = 80 * 1024;

    public SafeFileHandle GetFileHandle()
    {
        return File.OpenHandle(DestinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, FileOptions.RandomAccess | FileOptions.Asynchronous, ContentLength);
    }

    public async ValueTask DetectContentLengthAsync()
    {
        if (ContentLength > 0)
        {
            return;
        }

        HttpResponseMessage response = await HttpClient.HeadAsync(SourceUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        long contentLength = response.Content.Headers.ContentLength ?? 0;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(contentLength);
        ContentLength = contentLength;
    }
}