// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Collections.Frozen;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Mime;

namespace Snap.Hutao.ViewModel.Guide;

internal sealed partial class DownloadSummary : ObservableObject
{
    private static readonly FrozenSet<string?> AllowedMediaTypes =
    [
        MediaTypeNames.Application.Octet,
        MediaTypeNames.Application.Zip,
    ];

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly IImageCache imageCache;
    private readonly HttpClient httpClient;

    private readonly string fileUrl;
    private readonly IProgress<StreamCopyStatus> progress;

    public DownloadSummary(IServiceProvider serviceProvider, string fileName)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        httpRequestMessageBuilderFactory = serviceProvider.GetRequiredService<IHttpRequestMessageBuilderFactory>();
        httpClient = serviceProvider.GetRequiredService<HttpClient>();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(HutaoRuntime.UserAgent);
        imageCache = serviceProvider.GetRequiredService<IImageCache>();
        hutaoUserOptions = serviceProvider.GetRequiredService<HutaoUserOptions>();

        this.serviceProvider = serviceProvider;

        Filename = fileName;
        fileUrl = StaticResourcesEndpoints.StaticZip(fileName);

        progress = serviceProvider.GetRequiredService<IProgressFactory>().CreateForMainThread<StreamCopyStatus>(UpdateProgressStatus);
    }

    public string Filename { get; }

    [ObservableProperty]
    public partial string Description { get; private set; } = SH.ViewModelWelcomeDownloadSummaryDefault;

    [ObservableProperty]
    public partial double ProgressValue { get; set; }

    public async ValueTask<bool> DownloadAndExtractAsync()
    {
        ILogger<DownloadSummary> logger = serviceProvider.GetRequiredService<ILogger<DownloadSummary>>();
        try
        {
            int retryTimes = 0;
            while (retryTimes++ < 3)
            {
                // Download static zip should not set x-homa-token headers
                HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory
                    .Create()
                    .SetRequestUri(fileUrl)
                    .SetStaticResourceControlHeaders()
                    .Get();

                TimeSpan delay = default;
                using (HttpRequestMessage message = builder.HttpRequestMessage)
                {
                    using (HttpResponseMessage response = await httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        if (!AllowedMediaTypes.Contains(response.Content.Headers.ContentType?.MediaType))
                        {
                            logger.LogWarning("Download Static Zip failed, Content-Type is {Type}", response.Content.Headers.ContentType);
                            await taskContext.SwitchToMainThreadAsync();
                            Description = SH.ViewModelWelcomeDownloadSummaryContentTypeNotMatch;
                        }
                        else
                        {
                            long contentLength = response.Content.Headers.ContentLength ?? 0;
                            logger.LogInformation("Begin download, size: {length}", Converters.ToFileSizeString(contentLength));
                            using (Stream content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                using (TempFileStream temp = new(FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                {
                                    using (StreamCopyWorker worker = new(content, temp, contentLength))
                                    {
                                        await worker.CopyAsync(progress).ConfigureAwait(false);
                                    }

                                    ExtractFiles(temp);
                                    await taskContext.SwitchToMainThreadAsync();
                                    ProgressValue = 1;
                                    Description = SH.ViewModelWelcomeDownloadSummaryComplete;
                                    StaticResource.Fulfill(Filename);
                                    return true;
                                }
                            }
                        }

                        if (response.Headers.RetryAfter?.Delta is { } retryAfter)
                        {
                            delay = retryAfter;
                        }
                    }
                }

                await Task.Delay(delay).ConfigureAwait(false);
            }

            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Download static zip failed");

            await taskContext.SwitchToMainThreadAsync();
            Description = ex is HttpRequestException httpRequestEx
                ? $"{SH.ViewModelWelcomeDownloadSummaryException} - [HTTP '{httpRequestEx.StatusCode:D}'] [Error '{httpRequestEx.HttpRequestError}']"
                : $"{TypeNameHelper.GetTypeDisplayName(ex, false)}: {ex.Message}";
            return false;
        }
    }

    private void UpdateProgressStatus(StreamCopyStatus status)
    {
        Description = $"{Converters.ToFileSizeString(status.BytesReadSinceCopyStart)}/{Converters.ToFileSizeString(status.TotalBytes)}";
        ProgressValue = status.TotalBytes is 0 ? 0 : (double)status.BytesReadSinceCopyStart / status.TotalBytes;
    }

    private void ExtractFiles(Stream stream)
    {
        if (imageCache is not IImageCacheFilePathOperation imageCacheFilePathOperation)
        {
            throw HutaoException.InvalidCast<IImageCache, IImageCacheFilePathOperation>(nameof(imageCache));
        }

        using (ZipArchive archive = new(stream))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string destPath = imageCacheFilePathOperation.GetFileFromCategoryAndName(Filename, entry.FullName);
                try
                {
                    entry.ExtractToFile(destPath, true);
                }
                catch
                {
                    // Ignored
                }
            }
        }
    }
}