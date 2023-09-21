// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.View.Card;
using Snap.Hutao.View.Card.Primitive;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Home;

/// <summary>
/// 公告视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AnnouncementViewModel : Abstraction.ViewModel
{
    private readonly IHutaoAsAService hutaoAsAService;
    private readonly IAnnouncementService announcementService;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly ITaskContext taskContext;

    private AnnouncementWrapper? announcement;
    private string greetingText = SH.ViewPageHomeGreetingTextDefault;
    private ObservableCollection<Web.Hutao.HutaoAsAService.Announcement>? hutaoAnnouncements;
    private List<CardReference>? cards;

    /// <summary>
    /// 公告
    /// </summary>
    public AnnouncementWrapper? Announcement { get => announcement; set => SetProperty(ref announcement, value); }

    public ObservableCollection<Web.Hutao.HutaoAsAService.Announcement>? HutaoAnnouncements { get => hutaoAnnouncements; set => SetProperty(ref hutaoAnnouncements, value); }

    /// <summary>
    /// 用户选项
    /// </summary>
    public HutaoUserOptions UserOptions { get => hutaoUserOptions; }

    /// <summary>
    /// 欢迎语
    /// </summary>
    public string GreetingText { get => greetingText; set => SetProperty(ref greetingText, value); }

    public List<CardReference>? Cards { get => cards; set => SetProperty(ref cards, value); }

    protected override ValueTask<bool> InitializeUIAsync()
    {
        InitializeDashboard();
        InitializeInGameAnnouncementAsync().SafeForget();
        InitializeHutaoAnnouncementAsync().SafeForget();
        UpdateGreetingText();
        return ValueTask.FromResult(true);
    }

    private async ValueTask InitializeInGameAnnouncementAsync()
    {
        try
        {
            AnnouncementWrapper announcementWrapper = await announcementService.GetAnnouncementWrapperAsync(CancellationToken).ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            Announcement = announcementWrapper;
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async ValueTask InitializeHutaoAnnouncementAsync()
    {
        try
        {
            ObservableCollection<Web.Hutao.HutaoAsAService.Announcement> hutaoAnnouncements = await hutaoAsAService.GetHutaoAnnouncementCollectionAsync().ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            HutaoAnnouncements = hutaoAnnouncements;
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
                GreetingText = SH.ViewPageHomeGreetingTextCommon2.Format(LocalSetting.Get(SettingKeys.LaunchTimes, 0));
            }
        }
    }

    private void InitializeDashboard()
    {
        List<CardReference> result = new();

        if (LocalSetting.Get(SettingKeys.IsHomeCardLaunchGamePresented, true))
        {
            result.Add(new() { Card = new LaunchGameCard() });
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardGachaStatisticsPresented, true))
        {
            result.Add(new() { Card = new GachaStatisticsCard() });
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardAchievementPresented, true))
        {
            result.Add(new() { Card = new AchievementCard() });
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardDailyNotePresented, true))
        {
            result.Add(new() { Card = new DailyNoteCard() });
        }

        Cards = result;
    }
}