// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Collections.Frozen;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;

namespace Snap.Hutao.Core.Caching;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IImageCacheDownloadOperation))]
[HttpClient(HttpClientConfiguration.Default)]
[PrimaryHttpMessageHandler(MaxConnectionsPerServer = 8)]
internal sealed partial class ImageCacheDownloadOperation : IImageCacheDownloadOperation
{
    private static readonly FrozenDictionary<int, TimeSpan> DelayFromRetryCount = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(0, TimeSpan.FromSeconds(4)),
        KeyValuePair.Create(1, TimeSpan.FromSeconds(16)),
        KeyValuePair.Create(2, TimeSpan.FromSeconds(64)),
    ]);

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHttpClientFactory httpClientFactory;

    public async ValueTask DownloadFileAsync(Uri uri, string baseFile)
    {
        using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(ImageCacheDownloadOperation)))
        {
            await DownloadFileUsingHttpClientAsync(httpClient, uri, baseFile).ConfigureAwait(false);
        }

        if (!File.Exists(baseFile))
        {
            throw InternalImageCacheException.Throw($"Unable to download file from '{uri.OriginalString}'");
        }
    }

    private async ValueTask DownloadFileUsingHttpClientAsync(HttpClient httpClient, Uri uri, string baseFile)
    {
        int retryCount = 0;

        HttpRequestMessageBuilder requestMessageBuilder = httpRequestMessageBuilderFactory
            .Create()
            .SetRequestUri(uri)
            .SetStaticResourceControlHeadersIfRequired()
            .Get();

        while (retryCount < 3)
        {
            requestMessageBuilder.Resurrect();

            using (HttpRequestMessage requestMessage = requestMessageBuilder.HttpRequestMessage)
            {
                HttpResponseMessage responseMessage;
                try
                {
                    responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (HttpRequestExceptionHandling.TryHandle(requestMessageBuilder, ex, out _))
                    {
                        continue;
                    }

                    throw;
                }

                using (responseMessage)
                {
                    if (responseMessage.IsSuccessStatusCode)
                    {
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

                            using (fileStream)
                            {
                                try
                                {
                                    await httpStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                    await fileStream.FlushAsync().ConfigureAwait(false);
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

                                return;
                            }
                        }
                    }

                    switch (responseMessage.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            {
                                throw InternalImageCacheException.Throw($"Unable to download file from '{uri.OriginalString}'");
                            }

                        case HttpStatusCode.TooManyRequests:
                            {
                                TimeSpan delay = responseMessage.Headers.RetryAfter?.Delta ?? DelayFromRetryCount[retryCount];
                                await Task.Delay(delay).ConfigureAwait(false);
                                break;
                            }

                        default:
                            {
                                throw InternalImageCacheException.Throw($"Unexpected HTTP status code {responseMessage.StatusCode}");
                            }
                    }
                }
            }

            retryCount++;
        }
    }
}