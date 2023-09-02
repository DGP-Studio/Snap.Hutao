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
using Snap.Hutao.Web.Hutao.GachaLog;
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
    private readonly INavigationService navigationService;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IGachaLogHutaoCloudService hutaoCloudService;
    private readonly IHutaoUserService hutaoUserService;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly HutaoUserOptions options;

    private ObservableCollection<HutaoCloudEntryOperationViewModel>? uidOperations;

    /// <summary>
    /// Uid集合
    /// </summary>
    public ObservableCollection<HutaoCloudEntryOperationViewModel>? UidOperations { get => uidOperations; set => SetProperty(ref uidOperations, value); }

    /// <summary>
    /// 选项
    /// </summary>
    public HutaoUserOptions Options { get => options; }

    /// <summary>
    /// 获取记录命令
    /// </summary>
    internal ICommand RetrieveCommand { get; set; }

    /// <summary>
    /// 异步获取祈愿记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>祈愿记录</returns>
    internal async ValueTask<ValueResult<bool, Guid>> RetrieveAsync(string uid)
    {
        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelGachaLogRetrieveFromHutaoCloudProgress)
            .ConfigureAwait(false);

        using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
        {
            return await hutaoCloudService.RetrieveGachaItemsAsync(uid).ConfigureAwait(false);
        }
    }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        await hutaoUserService.InitializeAsync().ConfigureAwait(false);
        await RefreshUidCollectionAsync().ConfigureAwait(false);
        return true;
    }

    [Command("NavigateToAfdianSkuCommand")]
    private static async Task NavigateToAfdianSkuAsync()
    {
        await Windows.System.Launcher.LaunchUriAsync("https://afdian.net/item/80d3b9decf9011edb5f452540025c377".ToUri());
    }

    [Command("UploadCommand")]
    private async Task UploadAsync(GachaArchive? gachaArchive)
    {
        if (gachaArchive is not null)
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
        if (uid is not null)
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

    [Command("NavigateToSpiralAbyssRecordCommand")]
    private void NavigateToSpiralAbyssRecord()
    {
        navigationService.Navigate<View.Page.SpiralAbyssRecordPage>(INavigationAwaiter.Default);
    }

    private async ValueTask RefreshUidCollectionAsync()
    {
        if (Options.IsCloudServiceAllowed)
        {
            Response<List<GachaEntry>> resp = await hutaoCloudService.GetGachaEntriesAsync().ConfigureAwait(false);

            if (resp.IsOk())
            {
                ObservableCollection<HutaoCloudEntryOperationViewModel> collcetion = resp.Data
                    .SelectList(entry => new HutaoCloudEntryOperationViewModel(entry, RetrieveCommand, DeleteCommand))
                    .ToObservableCollection();

                await taskContext.SwitchToMainThreadAsync();
                UidOperations = collcetion;
            }
        }
    }
}