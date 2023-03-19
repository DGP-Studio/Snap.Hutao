// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Bits;
using Snap.Hutao.Core.Setting;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 欢迎视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class WelcomeViewModel : ObservableObject
{
    private readonly IServiceProvider serviceProvider;

    private ObservableCollection<DownloadSummary>? downloadSummaries;

    /// <summary>
    /// 构造一个新的欢迎视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public WelcomeViewModel(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        OpenUICommand = new AsyncRelayCommand(OpenUIAsync);
    }

    /// <summary>
    /// 下载信息
    /// </summary>
    public ObservableCollection<DownloadSummary>? DownloadSummaries { get => downloadSummaries; set => SetProperty(ref downloadSummaries, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    private async Task OpenUIAsync()
    {
        IEnumerable<DownloadSummary> downloadSummaries = GenerateStaticResourceDownloadTasks();

        DownloadSummaries = downloadSummaries.ToObservableCollection();

        // Cancel all previous created jobs
        serviceProvider.GetRequiredService<BitsManager>().CancelAllJobs();
        await Parallel.ForEachAsync(downloadSummaries, async (summary, token) =>
        {
            await summary.DownloadAndExtractAsync().ConfigureAwait(false);
            ThreadHelper.InvokeOnMainThread(() => DownloadSummaries.Remove(summary));
        }).ConfigureAwait(true);

        serviceProvider.GetRequiredService<IMessenger>().Send(new Message.WelcomeStateCompleteMessage());
        StaticResource.FulfillAllContracts();

        try
        {
            new ToastContentBuilder()
                .AddText(SH.ViewModelWelcomeDownloadCompleteTitle)
                .AddText(SH.ViewModelWelcomeDownloadCompleteMessage)
                .Show();
        }
        catch (COMException)
        {
            // 0x803E0105
        }
    }

    private IEnumerable<DownloadSummary> GenerateStaticResourceDownloadTasks()
    {
        Dictionary<string, DownloadSummary> downloadSummaries = new();

        if (StaticResource.IsContractUnfulfilled(SettingKeys.StaticResourceV1Contract))
        {
            downloadSummaries.TryAdd("Bg", new(serviceProvider, "Bg"));
            downloadSummaries.TryAdd("AvatarIcon", new(serviceProvider, "AvatarIcon"));
            downloadSummaries.TryAdd("GachaAvatarIcon", new(serviceProvider, "GachaAvatarIcon"));
            downloadSummaries.TryAdd("GachaAvatarImg", new(serviceProvider, "GachaAvatarImg"));
            downloadSummaries.TryAdd("EquipIcon", new(serviceProvider, "EquipIcon"));
            downloadSummaries.TryAdd("GachaEquipIcon", new(serviceProvider, "GachaEquipIcon"));
            downloadSummaries.TryAdd("NameCardPic", new(serviceProvider, "NameCardPic"));
            downloadSummaries.TryAdd("Skill", new(serviceProvider, "Skill"));
            downloadSummaries.TryAdd("Talent", new(serviceProvider, "Talent"));
        }

        if (StaticResource.IsContractUnfulfilled(SettingKeys.StaticResourceV2Contract))
        {
            downloadSummaries.TryAdd("AchievementIcon", new(serviceProvider, "AchievementIcon"));
            downloadSummaries.TryAdd("ItemIcon", new(serviceProvider, "ItemIcon"));
            downloadSummaries.TryAdd("IconElement", new(serviceProvider, "IconElement"));
            downloadSummaries.TryAdd("RelicIcon", new(serviceProvider, "RelicIcon"));
        }

        if (StaticResource.IsContractUnfulfilled(SettingKeys.StaticResourceV3Contract))
        {
            downloadSummaries.TryAdd("Skill", new(serviceProvider, "Skill"));
            downloadSummaries.TryAdd("Talent", new(serviceProvider, "Talent"));
        }

        if (StaticResource.IsContractUnfulfilled(SettingKeys.StaticResourceV4Contract))
        {
            downloadSummaries.TryAdd("AvatarIcon", new(serviceProvider, "AvatarIcon"));
        }

        return downloadSummaries.Select(x => x.Value);
    }

    /// <summary>
    /// 下载信息
    /// </summary>
    [SuppressMessage("", "CA1067")]
    internal sealed class DownloadSummary : ObservableObject, IEquatable<DownloadSummary>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly BitsManager bitsManager;
        private readonly string fileName;
        private readonly Uri fileUri;
        private readonly Progress<ProgressUpdateStatus> progress;
        private string description = SH.ViewModelWelcomeDownloadSummaryDefault;
        private double progressValue;

        /// <summary>
        /// 构造一个新的下载信息
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="fileName">压缩文件名称</param>
        public DownloadSummary(IServiceProvider serviceProvider, string fileName)
        {
            this.serviceProvider = serviceProvider;
            bitsManager = serviceProvider.GetRequiredService<BitsManager>();
            DisplayName = fileName;
            this.fileName = fileName;
            fileUri = new(Web.HutaoEndpoints.StaticZip(fileName));

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

        /// <inheritdoc/>
        public bool Equals(DownloadSummary? other)
        {
            return fileName == other?.fileName;
        }

        /// <summary>
        /// 异步下载并解压
        /// </summary>
        /// <returns>任务</returns>
        public async Task DownloadAndExtractAsync()
        {
            (bool isOk, TempFile file) = await bitsManager.DownloadAsync(fileUri, progress).ConfigureAwait(false);

            using (file)
            {
                await ThreadHelper.SwitchToMainThreadAsync();
                if (isOk && File.Exists(file.Path))
                {
                    ProgressValue = 1;
                    await ThreadHelper.SwitchToBackgroundAsync();
                    ExtractFiles(file.Path);
                    await ThreadHelper.SwitchToMainThreadAsync();
                    Description = SH.ViewModelWelcomeDownloadSummaryComplete;
                }
                else
                {
                    ProgressValue = 0;
                    Description = SH.ViewModelWelcomeDownloadSummaryException;
                }
            }
        }

        private void UpdateProgressStatus(ProgressUpdateStatus status)
        {
            Description = $"{Converters.ToFileSizeString(status.BytesRead)}/{Converters.ToFileSizeString(status.TotalBytes)}";
            ProgressValue = status.TotalBytes == 0 ? 0 : (double)status.BytesRead / status.TotalBytes;
        }

        private void ExtractFiles(string file)
        {
            IImageCacheFilePathOperation imageCache = serviceProvider.GetRequiredService<IImageCache>().As<IImageCacheFilePathOperation>()!;
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(file))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string destPath = imageCache.GetFilePathFromCategoryAndFileName(fileName, entry.FullName);
                        entry.ExtractToFile(destPath, true);
                    }
                }
            }
            catch (InvalidDataException)
            {
                // System.IO.InvalidDataException: End of Central Directory record could not be found.
                // Basically the file downloaded is corrupted, skip anyway.
            }
        }
    }
}