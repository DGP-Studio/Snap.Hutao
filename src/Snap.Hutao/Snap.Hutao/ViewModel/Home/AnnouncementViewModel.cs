// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Announcement;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Control.Card;
using Snap.Hutao.UI.Xaml.View.Card;
using Snap.Hutao.Web.Hoyolab.Bbs.Home;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Miyolive;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Snap.Hutao.ViewModel.Home;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class AnnouncementViewModel : Abstraction.ViewModel
{
    private readonly IAnnouncementService announcementService;
    private readonly IServiceProvider serviceProvider;
    private readonly IHutaoAsAService hutaoAsAService;
    private readonly CultureOptions cultureOptions;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    [GeneratedConstructor]
    public partial AnnouncementViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial AnnouncementWrapper? Announcement { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Web.Hutao.HutaoAsAService.Announcement>? HutaoAnnouncements { get; set; }

    [ObservableProperty]
    public partial string GreetingText { get; set; } = SH.ViewPageHomeGreetingTextDefault;

    [ObservableProperty]
    public partial ImmutableArray<CodeWrapper> RedeemCodes { get; set; } = [];

    [ObservableProperty]
    public partial List<CardReference>? Cards { get; set; }

    [GeneratedRegex("act_id=(.*?)&")]
    private static partial Regex ActIdExtractor { get; }

    protected override ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        InitializeDashboard();
        InitializeInGameAnnouncementAsync(token).SafeForget();
        InitializeHutaoAnnouncementAsync(token).SafeForget();
        InitializeMiyoliveCodeAsync(token).SafeForget();
        UpdateGreetingText();
        return ValueTask.FromResult(true);
    }

    [SuppressMessage("", "SH003")]
    private async Task InitializeInGameAnnouncementAsync(CancellationToken token)
    {
        try
        {
            AnnouncementWrapper? announcementWrapper = await announcementService.GetAnnouncementWrapperAsync(cultureOptions.LanguageCode, appOptions.Region.Value, token).ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            Announcement = announcementWrapper;
            DeferContentLoader?.Load("GameAnnouncementPivot");
        }
        catch (OperationCanceledException)
        {
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task InitializeHutaoAnnouncementAsync(CancellationToken token)
    {
        try
        {
            ObservableCollection<Web.Hutao.HutaoAsAService.Announcement> hutaoAnnouncements = await hutaoAsAService.GetHutaoAnnouncementCollectionAsync(token).ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            HutaoAnnouncements = hutaoAnnouncements;
            DeferContentLoader?.Load("HutaoAnnouncementControl");
        }
        catch (OperationCanceledException)
        {
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task InitializeMiyoliveCodeAsync(CancellationToken token)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { IsOversea: false } userAndUid)
            {
                // The oversea user can direct use their code on the official website.
                return;
            }

            IHomeClient homeClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IHomeClient>>()
                .CreateFor(userAndUid);

            Response<NewHomeNewInfo> newHomeInfoResponse = await homeClient.GetNewHomeInfoAsync(2, token).ConfigureAwait(false);

            if (!ResponseValidator.TryValidateWithoutUINotification(newHomeInfoResponse, out NewHomeNewInfo? newHomeInfo))
            {
                return;
            }

            Uri url;
            if (newHomeInfo.Lives is [{ Data.LiveUrl: { } url1 }, ..])
            {
                url = url1;
            }
            else if (newHomeInfo.Navigator.SingleOrDefault(nav => nav.Name.EqualsAny(["直播兑换码", "前瞻直播"], StringComparison.OrdinalIgnoreCase)) is { AppPath: { } url2 })
            {
                url = url2;
            }
            else
            {
                return;
            }

            if (ActIdExtractor.Match(url.OriginalString) is not { Success: true, Groups: [_, { Value: { } actId }, ..] })
            {
                return;
            }

            IMiyoliveClient miyoliveClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IMiyoliveClient>>()
                .CreateFor(userAndUid);

            Response<CodeListWrapper> codeListResponse = await miyoliveClient.RefreshCodeAsync(actId, token).ConfigureAwait(false);
            if (!ResponseValidator.TryValidateWithoutUINotification(codeListResponse, out CodeListWrapper? wrapper))
            {
                return;
            }

            ImmutableArray<CodeWrapper> wrappers = wrapper.CodeList.SelectAsArray(static wrapper => wrapper.WithTitle(wrapper.Title.DecodeHtml()));
            wrappers = [.. wrappers.Where(static wrapper => !string.IsNullOrEmpty(wrapper.Code))];
            if (wrappers.IsEmpty)
            {
                return;
            }

            await taskContext.SwitchToMainThreadAsync();
            RedeemCodes = wrappers;
        }
    }

    [Command("CopyCodeCommand")]
    private async Task CopyCodeToClipboardAsync(string? code)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Copy redeem code to ClipBoard", "AnnouncementPage.Command"));

        if (string.IsNullOrEmpty(code))
        {
            return;
        }

        await serviceProvider.GetRequiredService<IClipboardProvider>().SetTextAsync(code).ConfigureAwait(false);
        serviceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Success(SH.ViewPageAnnouncementRedeemCodeCopySucceed));
    }

    private void UpdateGreetingText()
    {
        // TODO avatar birthday override.
        int rand = Random.Shared.Next(0, 1000);

        if (rand is >= 0 and < 6)
        {
            GreetingText = SH.ViewPageHomeGreetingTextEasterEgg;
        }
        else if (rand is >= 6 and < 57)
        {
            // TODO: retrieve days
            // GreetingText = string.Format(SH.ViewPageHomeGreetingTextEpic1, 0);
        }
        else if (rand is >= 57 and < 1000)
        {
            rand = Random.Shared.Next(0, 2);
            if (rand is 0)
            {
                // TODO: impl game launch times
                // GreetingText = string.Format(SH.ViewPageHomeGreetingTextCommon1, 0);
            }
            else if (rand is 1)
            {
                GreetingText = SH.FormatViewPageHomeGreetingTextCommon2(LocalSetting.Get(SettingKeys.LaunchTimes, 0));
            }
        }
    }

    private void InitializeDashboard()
    {
        List<CardReference> result = [];

        if (LocalSetting.Get(SettingKeys.IsHomeCardLaunchGamePresented, true))
        {
            result.Add(CardReference.Create(new LaunchGameCard(serviceProvider), SettingKeys.HomeCardLaunchGameOrder));
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardGachaStatisticsPresented, true))
        {
            result.Add(CardReference.Create(new GachaStatisticsCard(serviceProvider), SettingKeys.HomeCardGachaStatisticsOrder));
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardAchievementPresented, true))
        {
            result.Add(CardReference.Create(new AchievementCard(serviceProvider), SettingKeys.HomeCardAchievementOrder));
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardDailyNotePresented, true))
        {
            result.Add(CardReference.Create(new DailyNoteCard(serviceProvider), SettingKeys.HomeCardDailyNoteOrder));
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardCalendarPresented, true))
        {
            result.Add(CardReference.Create(new CalendarCard(serviceProvider), SettingKeys.HomeCardCalendarOrder));
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardSignInPresented, true))
        {
            result.Add(CardReference.Create(new SignInCard(serviceProvider), SettingKeys.HomeCardSignInOrder));
        }

        Cards = result.SortBy(r => r.Order);
    }
}