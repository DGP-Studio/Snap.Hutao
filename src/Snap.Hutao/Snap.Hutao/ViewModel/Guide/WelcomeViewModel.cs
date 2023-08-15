// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Notifications;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.Setting;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel.Guide;

/// <summary>
/// 欢迎视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class WelcomeViewModel : ObservableObject
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private ObservableCollection<DownloadSummary>? downloadSummaries;

    /// <summary>
    /// 下载信息
    /// </summary>
    public ObservableCollection<DownloadSummary>? DownloadSummaries { get => downloadSummaries; set => SetProperty(ref downloadSummaries, value); }

    [Command("OpenUICommand")]
    private async Task OpenUIAsync()
    {
        IEnumerable<DownloadSummary> downloadSummaries = GenerateStaticResourceDownloadTasks();

        DownloadSummaries = downloadSummaries.ToObservableCollection();

        await Parallel.ForEachAsync(downloadSummaries, async (summary, token) =>
        {
            if (await summary.DownloadAndExtractAsync().ConfigureAwait(false))
            {
                taskContext.InvokeOnMainThread(() => DownloadSummaries.Remove(summary));
            }
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

        if (StaticResource.IsContractUnfulfilled(StaticResource.V1Contract))
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

        if (StaticResource.IsContractUnfulfilled(StaticResource.V2Contract))
        {
            downloadSummaries.TryAdd("AchievementIcon", new(serviceProvider, "AchievementIcon"));
            downloadSummaries.TryAdd("ItemIcon", new(serviceProvider, "ItemIcon"));
            downloadSummaries.TryAdd("IconElement", new(serviceProvider, "IconElement"));
            downloadSummaries.TryAdd("RelicIcon", new(serviceProvider, "RelicIcon"));
        }

        if (StaticResource.IsContractUnfulfilled(StaticResource.V3Contract))
        {
            downloadSummaries.TryAdd("Skill", new(serviceProvider, "Skill"));
            downloadSummaries.TryAdd("Talent", new(serviceProvider, "Talent"));
        }

        if (StaticResource.IsContractUnfulfilled(StaticResource.V4Contract))
        {
            downloadSummaries.TryAdd("AvatarIcon", new(serviceProvider, "AvatarIcon"));
        }

        if (StaticResource.IsContractUnfulfilled(StaticResource.V5Contract))
        {
            downloadSummaries.TryAdd("MonsterIcon", new(serviceProvider, "MonsterIcon"));
        }

        return downloadSummaries.Select(x => x.Value);
    }
}