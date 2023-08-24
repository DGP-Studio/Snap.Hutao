// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.IO;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace Snap.Hutao.ViewModel.Guide;

/// <summary>
/// 下载信息
/// </summary>
internal sealed class DownloadSummary : ObservableObject
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly IImageCache imageCache;
    private readonly HttpClient httpClient;
    private readonly string fileName;
    private readonly string fileUrl;
    private readonly Progress<StreamCopyStatus> progress;
    private string description = SH.ViewModelWelcomeDownloadSummaryDefault;
    private double progressValue;
    private long updateCount;

    /// <summary>
    /// 构造一个新的下载信息
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="fileName">压缩文件名称</param>
    public DownloadSummary(IServiceProvider serviceProvider, string fileName)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        httpClient = serviceProvider.GetRequiredService<HttpClient>();
        imageCache = serviceProvider.GetRequiredService<IImageCache>();
        RuntimeOptions hutaoOptions = serviceProvider.GetRequiredService<RuntimeOptions>();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(hutaoOptions.UserAgent);

        this.serviceProvider = serviceProvider;

        DisplayName = fileName;
        this.fileName = fileName;
        fileUrl = Web.HutaoEndpoints.StaticZip(fileName);

        progress = new(UpdateProgressStatus);
    }

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; init; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get => description; private set => SetProperty(ref description, value); }

    /// <summary>
    /// 进度值，最大1
    /// </summary>
    public double ProgressValue { get => progressValue; set => SetProperty(ref progressValue, value); }

    /// <summary>
    /// 异步下载并解压
    /// </summary>
    /// <returns>任务</returns>
    public async ValueTask<bool> DownloadAndExtractAsync()
    {
        ILogger<DownloadSummary> logger = serviceProvider.GetRequiredService<ILogger<DownloadSummary>>();
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            long contentLength = response.Content.Headers.ContentLength ?? 0;
            logger.LogInformation("Begin download, length: {length}", contentLength);
            using (Stream content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                using (TempFileStream temp = new(FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    await new StreamCopyWorker(content, temp, contentLength).CopyAsync(progress).ConfigureAwait(false);
                    ExtractFiles(temp);

                    await taskContext.SwitchToMainThreadAsync();
                    ProgressValue = 1;
                    Description = SH.ViewModelWelcomeDownloadSummaryComplete;
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Download Static Zip failed");
            await taskContext.SwitchToMainThreadAsync();
            Description = ex is HttpRequestException httpRequestException
                ? $"{SH.ViewModelWelcomeDownloadSummaryException} - HTTP {httpRequestException.StatusCode:D}"
                : SH.ViewModelWelcomeDownloadSummaryException;
            return false;
        }
    }

    private void UpdateProgressStatus(StreamCopyStatus status)
    {
        if (Interlocked.Increment(ref updateCount) % 40 == 0)
        {
            Description = $"{Converters.ToFileSizeString(status.BytesCopied)}/{Converters.ToFileSizeString(status.TotalBytes)}";
            ProgressValue = status.TotalBytes == 0 ? 0 : (double)status.BytesCopied / status.TotalBytes;
        }
    }

    private void ExtractFiles(Stream stream)
    {
        IImageCacheFilePathOperation? imageCacheFilePathOperation = imageCache.As<IImageCacheFilePathOperation>();
        ArgumentNullException.ThrowIfNull(imageCacheFilePathOperation);

        using (ZipArchive archive = new(stream))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string destPath = imageCacheFilePathOperation.GetFileFromCategoryAndName(fileName, entry.FullName);
                entry.ExtractToFile(destPath, true);
            }
        }
    }
}