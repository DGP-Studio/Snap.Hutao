// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Control;
using Snap.Hutao.Core;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.View.Page;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 公告视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class AnnouncementViewModel : ObservableObject, ISupportCancellation
{
    private readonly IAnnouncementService announcementService;

    private AnnouncementWrapper? announcement;

    /// <summary>
    /// 构造一个公告视图模型
    /// </summary>
    /// <param name="announcementService">公告服务</param>
    /// <param name="navigationService">导航服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="logger">日志器</param>
    public AnnouncementViewModel(IAnnouncementService announcementService, IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.announcementService = announcementService;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        OpenAnnouncementUICommand = new RelayCommand<string>(OpenAnnouncementUI);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 公告
    /// </summary>
    public AnnouncementWrapper? Announcement
    {
        get => announcement;
        set => SetProperty(ref announcement, value);
    }

    /// <summary>
    /// 打开界面触发的命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 打开公告UI触发的命令
    /// </summary>
    public ICommand OpenAnnouncementUICommand { get; }

    private async Task OpenUIAsync()
    {
        try
        {
            Announcement = await announcementService.GetAnnouncementsAsync(CancellationToken).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void OpenAnnouncementUI(string? content)
    {
        if (WebView2Helper.IsSupported)
        {
            INavigationService navigationService = Ioc.Default.GetRequiredService<INavigationService>();
            navigationService.Navigate<AnnouncementContentPage>(data: new NavigationExtra(content));
        }
        else
        {
            IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();
            infoBarService.Warning("尚未安装 WebView2 Runtime");
        }
    }
}
