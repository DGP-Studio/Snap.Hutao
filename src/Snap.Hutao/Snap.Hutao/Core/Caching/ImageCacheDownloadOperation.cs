// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.ExceptionService;
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
    private readonly IServiceScopeFactory serviceScopeFactory;

    public async ValueTask DownloadFileAsync(Uri uri, string baseFile)
    {
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(ImageCacheDownloadOperation)))
            {
                await DownloadFileUsingHttpClientAsync(httpClient, uri, baseFile).ConfigureAwait(false);
            }
        }

        if (!File.Exists(baseFile))
        {
            throw HutaoException.InvalidOperation($"Unable to download file from '{uri.OriginalString}'", HutaoException.Marker);
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
                            Directory.CreateDirectory(directoryName);

                            using (FileStream fileStream = File.Create(baseFile))
                            {
                                await httpStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                return;
                            }
                        }
                    }

                    switch (responseMessage.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            {
                                throw HutaoException.InvalidOperation($"Unable to download file from '{uri.OriginalString}'", HutaoException.Marker);
                            }

                        case HttpStatusCode.TooManyRequests:
                            {
                                TimeSpan delay = responseMessage.Headers.RetryAfter?.Delta ?? DelayFromRetryCount[retryCount];
                                await Task.Delay(delay).ConfigureAwait(false);
                                break;
                            }
                    }
                }
            }

            retryCount++;
        }
    }
}