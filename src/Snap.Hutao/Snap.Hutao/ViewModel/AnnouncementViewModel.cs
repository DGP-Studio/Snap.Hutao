// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 公告视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class AnnouncementViewModel : Abstraction.ViewModel
{
    private readonly IAnnouncementService announcementService;

    private AnnouncementWrapper? announcement;

    /// <summary>
    /// 构造一个公告视图模型
    /// </summary>
    /// <param name="announcementService">公告服务</param>
    public AnnouncementViewModel(IAnnouncementService announcementService)
    {
        this.announcementService = announcementService;

        OpenUICommand = new AsyncRelayCommand(OpenUIAsync);
    }

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
}
