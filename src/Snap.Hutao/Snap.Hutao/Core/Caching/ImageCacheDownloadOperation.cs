// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;

namespace Snap.Hutao.Core.Caching;

[ConstructorGenerated]
[Service(ServiceLifetime.Singleton, typeof(IImageCacheDownloadOperation))]
[HttpClient(HttpClientConfiguration.Default)]
[PrimaryHttpMessageHandler(MaxConnectionsPerServer = 8)]
internal sealed partial class ImageCacheDownloadOperation : IImageCacheDownloadOperation
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHttpClientFactory httpClientFactory;

    public async ValueTask DownloadFileAsync(Uri uri, string baseFile)
    {
        using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(ImageCacheDownloadOperation)))
        {
            try
            {
                await DownloadFileUsingHttpClientAsync(httpClient, uri, baseFile).ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                // Ignore
            }
        }

        if (!File.Exists(baseFile))
        {
            throw InternalImageCacheException.Throw($"Unable to download file from '{uri.OriginalString}'");
        }
    }

    private async ValueTask DownloadFileUsingHttpClientAsync(HttpClient httpClient, Uri uri, string baseFile)
    {
        HttpRequestMessageBuilder requestMessageBuilder = httpRequestMessageBuilderFactory
            .Create()
            .SetRequestUri(uri)
            .SetStaticResourceControlHeadersIfRequired()
            .Get();

        using (HttpRequestMessage requestMessage = requestMessageBuilder.HttpRequestMessage)
        {
            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using (responseMessage)
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw responseMessage.StatusCode switch
                    {
                        HttpStatusCode.NotFound => InternalImageCacheException.Throw($"Unable to download file from '{uri.OriginalString}'"),
                        _ => InternalImageCacheException.Throw($"Unexpected HTTP status code {responseMessage.StatusCode}"),
                    };
                }

                if (responseMessage.Content.Headers.ContentType?.MediaType is MediaTypeNames.Application.Json)
                {
                    return;
                }

                using (Stream httpStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    string? directoryName = Path.GetDirectoryName(baseFile);
                    ArgumentException.ThrowIfNullOrEmpty(directoryName);
                    try
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        throw InternalImageCacheException.Throw($"Unable to create folder at '{directoryName}'");
                    }

                    FileStream fileStream;
                    try
                    {
                        fileStream = File.Create(baseFile);
                    }
                    catch (IOException ex)
                    {
                        // The process cannot access the file '?' because it is being used by another process.
                        throw InternalImageCacheException.Throw($"Unable to create file at '{baseFile}'", ex);
                    }

                    try
                    {
                        using (fileStream)
                        {
                            await httpStream.CopyToAsync(fileStream).ConfigureAwait(false);
                        }
                    }
                    catch (IOException ex)
                    {
                        // Received an unexpected EOF or 0 bytes from the transport stream.
                        // Unable to read data from the transport connection: 远程主机强迫关闭了一个现有的连接。. SocketException: ConnectionReset
                        // Unable to read data from the transport connection: 你的主机中的软件中止了一个已建立的连接。. SocketException: ConnectionAborted
                        // HttpIOException: The response ended prematurely. (ResponseEnded)
                        // 磁盘空间不足。 : '?'.
                        throw InternalImageCacheException.Throw("Unable to copy stream content to file", ex);
                    }
                }
            }
        }
    }
}