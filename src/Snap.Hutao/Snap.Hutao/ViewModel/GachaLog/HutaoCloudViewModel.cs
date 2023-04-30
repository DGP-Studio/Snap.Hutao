// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Web.Response;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 胡桃云服务视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal sealed class HutaoCloudViewModel : Abstraction.ViewModel
{
    private readonly ITaskContext taskContext;
    private readonly IHutaoCloudService hutaoCloudService;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IInfoBarService infoBarService;
    private readonly IServiceProvider serviceProvider;

    private ObservableCollection<string>? uids;

    /// <summary>
    /// 构造一个新的胡桃云服务视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public HutaoCloudViewModel(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        hutaoCloudService = serviceProvider.GetRequiredService<IHutaoCloudService>();
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        Options = serviceProvider.GetRequiredService<HutaoUserOptions>();
        this.serviceProvider = serviceProvider;

        UploadCommand = new AsyncRelayCommand<GachaArchive>(UploadAsync);
        DeleteCommand = new AsyncRelayCommand<string>(DeleteAsync);
        NavigateToSpiralAbyssRecordCommand = new RelayCommand(NavigateToSpiralAbyssRecord);
        NavigateToAfdianSKuCommand = new AsyncRelayCommand(NavigateToAfdianSKuAsync);
    }

    /// <summary>
    /// Uid集合
    /// </summary>
    public ObservableCollection<string>? Uids { get => uids; set => SetProperty(ref uids, value); }

    /// <summary>
    /// 选项
    /// </summary>
    public HutaoUserOptions Options { get; }

    /// <summary>
    /// 上传记录命令
    /// </summary>
    public ICommand UploadCommand { get; }

    /// <summary>
    /// 删除云端记录
    /// </summary>
    public ICommand DeleteCommand { get; }

    /// <summary>
    /// 导航到深渊记录页面
    /// </summary>
    public ICommand NavigateToSpiralAbyssRecordCommand { get; }

    /// <summary>
    /// 导航到爱发电
    /// </summary>
    public ICommand NavigateToAfdianSKuCommand { get; }

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

    private void NavigateToSpiralAbyssRecord()
    {
        serviceProvider
            .GetRequiredService<INavigationService>()
            .Navigate<View.Page.SpiralAbyssRecordPage>(INavigationAwaiter.Default);
    }

    private async Task NavigateToAfdianSKuAsync()
    {
        await Windows.System.Launcher.LaunchUriAsync(new(@"ms-windows-store://pdp/?productid=9PH4NXJ2JN52"));
    }
}