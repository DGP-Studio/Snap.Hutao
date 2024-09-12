// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Web.Request.Builder;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Core.IO.Http.Sharding;

internal sealed class HttpShardCopyWorkerOptions<TStatus>
{
    private bool isReadOnly;
    private HttpClient httpClient = default!;
    private string sourceUrl = default!;
    private string destinationFilePath = default!;
    private long contentLength;
    private StreamCopyStatusFactory<TStatus> statusFactory = default!;
    private int bufferSize = 80 * 1024;
    private int maxDegreeOfParallelism = Environment.ProcessorCount;

    private SafeFileHandle? destinationFileHandle;

    public HttpClient HttpClient
    {
        get => httpClient;
        set
        {
            VerifyMutable();
            httpClient = value;
        }
    }

    public string SourceUrl
    {
        get => sourceUrl;
        set
        {
            VerifyMutable();
            sourceUrl = value;
        }
    }

    public string DestinationFilePath
    {
        get => destinationFilePath;
        set
        {
            VerifyMutable();
            destinationFilePath = value;
        }
    }

    public SafeFileHandle DestinationFileHandle { get => destinationFileHandle ??= GetFileHandle(); }

    public long ContentLength
    {
        get => contentLength;
        private set
        {
            VerifyMutable();
            contentLength = value;
        }
    }

    public StreamCopyStatusFactory<TStatus> StatusFactory
    {
        get => statusFactory;
        set
        {
            VerifyMutable();
            statusFactory = value;
        }
    }

    public int BufferSize
    {
        get => bufferSize;
        set
        {
            VerifyMutable();
            bufferSize = value;
        }
    }

    public int MaxDegreeOfParallelism
    {
        get => maxDegreeOfParallelism;
        set
        {
            VerifyMutable();
            maxDegreeOfParallelism = value;
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

        long contentLength = response.Content.Headers.ContentLength ?? 0;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(contentLength);
        ContentLength = contentLength;
    }

    public void MakeReadOnly()
    {
        isReadOnly = true;
    }

    private SafeFileHandle GetFileHandle()
    {
        return File.OpenHandle(DestinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, FileOptions.RandomAccess | FileOptions.Asynchronous, ContentLength);
    }

    private void VerifyMutable()
    {
        Verify.Operation(!isReadOnly, "The options is read-only.");
    }
}