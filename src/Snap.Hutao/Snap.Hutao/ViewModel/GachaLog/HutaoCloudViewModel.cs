// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Response;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 胡桃云服务视图模型
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class HutaoCloudViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IHutaoCloudService hutaoCloudService;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly HutaoUserOptions options;

    private ObservableCollection<string>? uids;

    /// <summary>
    /// Uid集合
    /// </summary>
    public ObservableCollection<string>? Uids { get => uids; set => SetProperty(ref uids, value); }

    /// <summary>
    /// 选项
    /// </summary>
    public HutaoUserOptions Options { get => options; }

    /// <summary>
    /// 异步获取祈愿记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>祈愿记录</returns>
    public async Task<ValueResult<bool, GachaArchive?>> RetrieveAsync(string uid)
    {
        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelGachaLogRetrieveFromHutaoCloudProgress)
            .ConfigureAwait(false);

        using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
        {
            return await hutaoCloudService.RetrieveGachaItemsAsync(uid).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        await serviceProvider.GetRequiredService<IHutaoUserService>().InitializeAsync().ConfigureAwait(false);
        await RefreshUidCollectionAsync().ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        IsInitialized = true;
    }

    [Command("UploadCommand")]
    private async Task UploadAsync(GachaArchive? gachaArchive)
    {
        if (gachaArchive != null)
        {
            ContentDialog dialog = await contentDialogFactory
                .CreateForIndeterminateProgressAsync(SH.ViewModelGachaLogUploadToHutaoCloudProgress)
                .ConfigureAwait(false);

            bool isOk;
            string message;

            using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
            {
                (isOk, message) = await hutaoCloudService.UploadGachaItemsAsync(gachaArchive).ConfigureAwait(false);
            }

            if (isOk)
            {
                infoBarService.Success(message);
                await RefreshUidCollectionAsync().ConfigureAwait(false);
            }
            else
            {
                infoBarService.Warning(message);
            }
        }
    }

    [Command("DeleteCommand")]
    private async Task DeleteAsync(string? uid)
    {
        if (uid != null)
        {
            (bool isOk, string message) = await hutaoCloudService.DeleteGachaItemsAsync(uid).ConfigureAwait(false);

            if (isOk)
            {
                infoBarService.Success(message);
                await RefreshUidCollectionAsync().ConfigureAwait(false);
            }
            else
            {
                infoBarService.Warning(message);
            }
        }
    }

    private async Task RefreshUidCollectionAsync()
    {
        Response<List<string>> resp = await hutaoCloudService.GetUidsAsync().ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        if (Options.IsCloudServiceAllowed = resp.IsOk())
        {
            Uids = resp.Data!.ToObservableCollection();
        }
    }

    [Command("NavigateToSpiralAbyssRecordCommand")]
    private void NavigateToSpiralAbyssRecord()
    {
        serviceProvider
            .GetRequiredService<INavigationService>()
            .Navigate<View.Page.SpiralAbyssRecordPage>(INavigationAwaiter.Default);
    }

    [Command("NavigateToAfdianSkuCommand")]
    private async Task NavigateToAfdianSkuAsync()
    {
        await Windows.System.Launcher.LaunchUriAsync(new(@"ms-windows-store://pdp/?productid=9PH4NXJ2JN52"));
    }
}