// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Hutao;
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
    private readonly ILogger<ImageCacheDownloadOperation> logger;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly HutaoUserOptions hutaoUserOptions;

    public async ValueTask DownloadFileAsync(Uri uri, string baseFile)
    {
        using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(ImageCacheDownloadOperation)))
        {
            int retryCount = 0;

            string authority = uri.GetLeftPart(UriPartial.Authority);
            bool shouldAddControlHeader = authority.Equals(StaticResourcesEndpoints.Root, StringComparison.OrdinalIgnoreCase);

            HttpRequestMessageBuilder requestMessageBuilder = httpRequestMessageBuilderFactory
                .Create()
                .SetRequestUri(uri)
                .SetStaticResourceControlHeadersIf(shouldAddControlHeader)
                .Get();

            await requestMessageBuilder.InfrastructureSetTraceInfoAsync(hutaoUserOptions).ConfigureAwait(false);

            while (retryCount < 3)
            {
                requestMessageBuilder.Resurrect();

                using (HttpRequestMessage requestMessage = requestMessageBuilder.HttpRequestMessage)
                {
                    using (HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        // Redirect detection
                        if (responseMessage.RequestMessage is { RequestUri: { } target } && target != uri)
                        {
                            const string Template = """
                                The Request to
                                '{Source}'
                                has been redirected to
                                '{Target}'
                                """;
                            logger.LogDebug(Template, uri, target);
                        }

                        if (responseMessage.IsSuccessStatusCode)
                        {
                            if (responseMessage.Content.Headers.ContentType?.MediaType is MediaTypeNames.Application.Json)
                            {
                                string raw = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                                logger.LogCritical("Failed to download '\e[1m\e[31m{Uri}\e[37m' with unexpected body '\e[33m{Raw}\e[37m'", uri, raw);
                                return;
                            }

                            using (Stream httpStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                using (FileStream fileStream = File.Create(baseFile))
                                {
                                    await httpStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                    return;
                                }
                            }
                        }

                        switch (responseMessage.StatusCode)
                        {
                            case HttpStatusCode.TooManyRequests:
                                {
                                    retryCount++;
                                    TimeSpan delay = responseMessage.Headers.RetryAfter?.Delta ?? DelayFromRetryCount[retryCount];
                                    logger.LogInformation("Retry download '{Uri}' after {Delay}.", uri, delay);
                                    await Task.Delay(delay).ConfigureAwait(false);
                                    break;
                                }

                            default:
                                logger.LogCritical("Failed to download '\e[1m\e[31m{Uri}\e[37m' with status code '\e[33m{StatusCode}\e[37m'", uri, responseMessage.StatusCode);
                                return;
                        }
                    }
                }
            }
        }
    }
}