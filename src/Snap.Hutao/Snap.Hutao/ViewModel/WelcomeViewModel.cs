// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Bits;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Extension;
using Snap.Hutao.Factory.Abstraction;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 欢迎视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class WelcomeViewModel : ObservableObject
{
    private readonly IServiceProvider serviceProvider;

    private ObservableCollection<DownloadSummary>? downloadSummaries;

    /// <summary>
    /// 构造一个新的欢迎视图模型
    /// </summary>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="serviceProvider">服务提供器</param>
    public WelcomeViewModel(IAsyncRelayCommandFactory asyncRelayCommandFactory, IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
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
        List<DownloadSummary> downloadSummaries = new();

        if (!LocalSetting.Get(SettingKeys.StaticResourceV1Contract, false))
        {
            downloadSummaries.Add(new(serviceProvider, "基础图标", "Bg"));
            downloadSummaries.Add(new(serviceProvider, "角色图标", "AvatarIcon"));
            downloadSummaries.Add(new(serviceProvider, "角色立绘图标", "GachaAvatarIcon"));
            downloadSummaries.Add(new(serviceProvider, "角色立绘图像", "GachaAvatarImg"));
            downloadSummaries.Add(new(serviceProvider, "武器图标", "EquipIcon"));
            downloadSummaries.Add(new(serviceProvider, "武器立绘图标", "GachaEquipIcon"));
            downloadSummaries.Add(new(serviceProvider, "名片图像", "NameCardPic"));
            downloadSummaries.Add(new(serviceProvider, "天赋图标", "Skill"));
            downloadSummaries.Add(new(serviceProvider, "命之座图标", "Talent"));
        }

        if (!LocalSetting.Get(SettingKeys.StaticResourceV2Contract, false))
        {
            downloadSummaries.Add(new(serviceProvider, "成就图标", "AchievementIcon"));
            downloadSummaries.Add(new(serviceProvider, "物品图标", "ItemIcon"));
            downloadSummaries.Add(new(serviceProvider, "元素图标", "IconElement"));
        }

        DownloadSummaries = new(downloadSummaries);

        await Task.WhenAll(downloadSummaries.Select(d => d.DownloadAndExtractAsync())).ConfigureAwait(true);

        serviceProvider.GetRequiredService<IMessenger>().Send(new Message.WelcomeStateCompleteMessage());

        // Complete StaticResourceContracts
        LocalSetting.Set(SettingKeys.StaticResourceV1Contract, true);
        LocalSetting.Set(SettingKeys.StaticResourceV2Contract, true);

        new ToastContentBuilder()
            .AddText("下载完成")
            .AddText("现在可以开始使用胡桃了")
            .Show();
    }

    /// <summary>
    /// 下载信息
    /// </summary>
    public class DownloadSummary : ObservableObject
    {
        private readonly IServiceProvider serviceProvider;
        private readonly BitsManager bitsManager;
        private readonly string fileName;
        private readonly Uri fileUri;
        private readonly Progress<ProgressUpdateStatus> progress;
        private string description = "等待中";
        private double progressValue;

        /// <summary>
        /// 构造一个新的下载信息
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="displayName">显示名称</param>
        /// <param name="fileName">压缩文件名称</param>
        public DownloadSummary(IServiceProvider serviceProvider, string displayName, string fileName)
        {
            this.serviceProvider = serviceProvider;
            bitsManager = serviceProvider.GetRequiredService<BitsManager>();
            DisplayName = displayName;
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
                    Description = "完成";
                }
                else
                {
                    ProgressValue = 0;
                    Description = "文件下载异常";
                }
            }
        }

        private void UpdateProgressStatus(ProgressUpdateStatus status)
        {
            Description = $"{Converters.ToFileSizeString(status.BytesRead)}/{Converters.ToFileSizeString(status.TotalBytes)}";
            ProgressValue = (double)status.BytesRead / status.TotalBytes;
        }

        private void ExtractFiles(string file)
        {
            IImageCacheFilePathOperation imageCache = serviceProvider.GetRequiredService<IImageCache>().ImplictAs<IImageCacheFilePathOperation>()!;

            using (ZipArchive archive = ZipFile.OpenRead(file))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string destPath = imageCache.GetFilePathFromCategoryAndFileName(fileName, entry.FullName);
                    entry.ExtractToFile(destPath, true);
                }
            }
        }
    }
}