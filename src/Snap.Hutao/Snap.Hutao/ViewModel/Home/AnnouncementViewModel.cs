// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

namespace Snap.Hutao.ViewModel.Home;

/// <summary>
/// 公告视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AnnouncementViewModel : Abstraction.ViewModel
{
    private readonly IAnnouncementService announcementService;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly ITaskContext taskContext;

    private AnnouncementWrapper? announcement;
    private string greetingText = SH.ViewPageHomeGreetingTextDefault;

    /// <summary>
    /// 公告
    /// </summary>
    public AnnouncementWrapper? Announcement { get => announcement; set => SetProperty(ref announcement, value); }

    /// <summary>
    /// 用户选项
    /// </summary>
    public HutaoUserOptions UserOptions { get => hutaoUserOptions; }

    /// <summary>
    /// 欢迎语
    /// </summary>
    public string GreetingText { get => greetingText; set => SetProperty(ref greetingText, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        try
        {
            AnnouncementWrapper announcement = await announcementService.GetAnnouncementsAsync(CancellationToken).ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            Announcement = announcement;
            UpdateGreetingText();
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void UpdateGreetingText()
    {
        // TODO avatar birthday override.
        int rand = Random.Shared.Next(0, 1000);

        if (rand >= 0 && rand < 6)
        {
            GreetingText = SH.ViewPageHomeGreetingTextEasterEgg;
        }
        else if (rand >= 6 && rand < 57)
        {
            // TODO: retrieve days
            // GreetingText = string.Format(SH.ViewPageHomeGreetingTextEpic1, 0);
        }
        else if (rand >= 57 && rand < 1000)
        {
            rand = Random.Shared.Next(0, 2);
            if (rand == 0)
            {
                // TODO: impl game launch times
                // GreetingText = string.Format(SH.ViewPageHomeGreetingTextCommon1, 0);
            }
            else if (rand == 1)
            {
                GreetingText = string.Format(SH.ViewPageHomeGreetingTextCommon2, LocalSetting.Get(SettingKeys.LaunchTimes, 0));
            }
        }
    }
}