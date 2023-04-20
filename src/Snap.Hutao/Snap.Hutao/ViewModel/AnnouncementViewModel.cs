// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 公告视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class AnnouncementViewModel : Abstraction.ViewModel
{
    private readonly IAnnouncementService announcementService;

    private AnnouncementWrapper? announcement;

    /// <summary>
    /// 构造一个公告视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public AnnouncementViewModel(IServiceProvider serviceProvider)
    {
        announcementService = serviceProvider.GetRequiredService<IAnnouncementService>();
    }

    /// <summary>
    /// 公告
    /// </summary>
    public AnnouncementWrapper? Announcement { get => announcement; set => SetProperty(ref announcement, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
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