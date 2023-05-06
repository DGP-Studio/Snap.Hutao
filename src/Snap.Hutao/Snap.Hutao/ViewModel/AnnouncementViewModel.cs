// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 公告视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AnnouncementViewModel : Abstraction.ViewModel
{
    private readonly IAnnouncementService announcementService;

    private AnnouncementWrapper? announcement;

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