// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Collections.Frozen;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Mime;
using System.Text;

namespace Snap.Hutao.ViewModel.Guide;

internal sealed partial class DownloadSummary : ObservableObject
{
    private static readonly FrozenSet<string?> AllowedMediaTypes =
    [
        MediaTypeNames.Application.Octet,
        MediaTypeNames.Application.Zip,
    ];

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IInfoBarService infoBarService;
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
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        FileName = fileName;

        fileUrl = StaticResourcesEndpoints.StaticZip(fileName);
        progress = serviceProvider.GetRequiredService<IProgressFactory>().CreateForMainThread<StreamCopyStatus>(UpdateProgressStatus);
    }

    public string FileName { get; }

    [ObservableProperty]
    public partial string Description { get; private set; } = SH.ViewModelWelcomeDownloadSummaryDefault;

    [ObservableProperty]
    public partial double ProgressValue { get; set; }

    public async ValueTask<bool> DownloadAndExtractAsync()
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory
            .Create()
            .SetRequestUri(fileUrl)
            .SetStaticResourceControlHeaders()
            .Get();

        try
        {
            int retryTimes = 0;
            while (retryTimes++ < 3)
            {
                builder.Resurrect();

                TimeSpan delay = default;
                using (HttpRequestMessage message = builder.HttpRequestMessage)
                {
                    using (HttpResponseMessage response = await httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();

                        if (!AllowedMediaTypes.Contains(response.Content.Headers.ContentType?.MediaType))
                        {
                            await taskContext.SwitchToMainThreadAsync();
                            Description = SH.ViewModelWelcomeDownloadSummaryContentTypeNotMatch;
                        }
                        else
                        {
                            long contentLength = response.Content.Headers.ContentLength ?? 0;
                            using (Stream content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                using (TempFileStream tempStream = new(FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                {
                                    using (StreamCopyWorker worker = new(content, tempStream, contentLength))
                                    {
                                        await worker.CopyAsync(progress).ConfigureAwait(false);
                                    }

                                    ExtractFiles(tempStream);

                                    await taskContext.SwitchToMainThreadAsync();
                                    ProgressValue = 1;
                                    Description = SH.ViewModelWelcomeDownloadSummaryComplete;
                                    StaticResource.Fulfill(FileName);
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
            if (HttpRequestExceptionHandling.TryHandle(builder, ex, out StringBuilder message))
            {
                infoBarService.Error(SH.ViewModelWelcomeDownloadSummaryException, message.ToString());
            }
            else
            {
                infoBarService.Error(ex, SH.ViewModelWelcomeDownloadSummaryException);
            }

            await taskContext.SwitchToMainThreadAsync();
            Description = SH.ViewModelWelcomeDownloadSummaryException;
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
        using (ZipArchive archive = new(stream))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string destPath = imageCache.GetFileFromCategoryAndName(FileName, entry.FullName);

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