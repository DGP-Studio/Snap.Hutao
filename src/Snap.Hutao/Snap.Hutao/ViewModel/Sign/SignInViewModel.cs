// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.SignIn;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Sign;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class SignInViewModel : Abstraction.ViewModelSlim, IRecipient<UserAndUidChangedMessage>
{
    private readonly WeakReference<ScrollViewer> weakScrollViewer = new(default!);

    private readonly IContentDialogFactory contentDialogFactory;
    private readonly CultureOptions cultureOptions;
    private readonly ISignInService signInService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IMessenger messenger;

    private bool updating;
    private int totalSignDay;
    private SignInRewardReSignInfo? resignInfo;

    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial SignInViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial IAdvancedCollectionView<AwardView>? Awards { get; set; }

    [ObservableProperty]
    public partial string? TotalSignInDaysHint { get; set; }

    [ObservableProperty]
    public partial string? CurrentUid { get; set; }

    [ObservableProperty]
    public partial bool IsTodaySigned { get; set; }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (Volatile.Read(ref updating))
        {
            return;
        }

        try
        {
            IsInitialized = false;
        }
        catch (ObjectDisposedException)
        {
            // Cannot access a disposed object. Object name: 'ObjectReference'.
            return;
        }

        if (message.UserAndUid is { } userAndUid)
        {
            UpdateSignInInfoAsync(userAndUid).SafeForget();
        }
        else
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
        }
    }

    public void AttachXamlElement(ScrollViewer scrollViewer)
    {
        weakScrollViewer.SetTarget(scrollViewer);
    }

    protected override async Task LoadAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            await UpdateSignInInfoAsync(userAndUid).ConfigureAwait(false);
        }
        else
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
        }
    }

    private static void InitializeClaimedAwards(ImmutableArray<AwardView> awards, int totalSignDay)
    {
        foreach (ref readonly AwardView awardView in awards.AsSpan(..totalSignDay))
        {
            awardView.IsClaimed = true;
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task UpdateSignInInfoAsync(UserAndUid userAndUid, bool postSign = false, bool postResign = false)
    {
        if (Interlocked.Exchange(ref updating, true))
        {
            return;
        }

        try
        {
            await taskContext.SwitchToBackgroundAsync();

            Reward? reward;
            SignInRewardInfo? info;
            SignInRewardReSignInfo? resignInfo;
            using (IServiceScope scope = ServiceProvider.CreateScope())
            {
                ISignInClient signInClient = scope.ServiceProvider
                    .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
                    .Create(userAndUid.IsOversea);

                Response<Reward> rewardResponse = await signInClient.GetRewardAsync(userAndUid.User).ConfigureAwait(false);
                if (!ResponseValidator.TryValidate(rewardResponse, scope.ServiceProvider, out reward))
                {
                    messenger.Send(InfoBarMessage.Error(SH.ServiceSignInRewardListRequestFailed));
                    return;
                }

                Response<SignInRewardInfo> infoResponse = await signInClient.GetInfoAsync(userAndUid).ConfigureAwait(false);
                if (!ResponseValidator.TryValidate(infoResponse, scope.ServiceProvider, out info))
                {
                    messenger.Send(InfoBarMessage.Error(SH.ServiceSignInInfoRequestFailed));
                    return;
                }

                Response<SignInRewardReSignInfo> resignInfoResponse = await signInClient.GetResignInfoAsync(userAndUid).ConfigureAwait(false);
                if (!ResponseValidator.TryValidate(resignInfoResponse, scope.ServiceProvider, out resignInfo))
                {
                    messenger.Send(InfoBarMessage.Error(SH.ServiceSignInInfoRequestFailed));
                    return;
                }
            }

            if (postSign || postResign)
            {
                Award award = reward.Awards[Math.Clamp(info.TotalSignDay - 1, 0, reward.Awards.Length - 1)];

                if (postSign)
                {
                    messenger.Send(InfoBarMessage.Success(SH.FormatServiceSignInSuccessReward(award.Name, award.Count)));
                }
                else if (postResign)
                {
                    messenger.Send(InfoBarMessage.Success(SH.FormatServiceReSignInSuccessReward(award.Name, award.Count)));
                }
            }

            ImmutableArray<AwardView> views = reward.Awards.SelectAsArray(AwardView.Create);
            InitializeClaimedAwards(views, info.TotalSignDay);

            totalSignDay = info.TotalSignDay;
            this.resignInfo = resignInfo;

            IAdvancedCollectionView<AwardView> advancedViews = views.AsAdvancedCollectionView();
            await taskContext.SwitchToMainThreadAsync();
            IsTodaySigned = info.IsSign;
            Awards = advancedViews;
            CurrentUid = userAndUid.Uid.ToString();

            string monthName = cultureOptions.CurrentCulture.Value.DateTimeFormat.MonthNames[reward.Month - 1];
            TotalSignInDaysHint = SH.FormatViewModelSignInTotalSignInDaysHint(monthName, info.TotalSignDay);
            ScrollToCurrentOrNextAward(postSign || postResign);

            IsInitialized = true;
        }
        catch (ObjectDisposedException)
        {
            // Ignore
        }
        finally
        {
            Volatile.Write(ref updating, false);
        }
    }

    [Command("ScrollToNextAwardCommand")]
    private void ScrollToNextAward()
    {
        ScrollToCurrentOrNextAward();
    }

    private void ScrollToCurrentOrNextAward(bool current = false)
    {
        if (!weakScrollViewer.TryGetTarget(out ScrollViewer? scrollViewer))
        {
            return;
        }

        DateTime now = DateTime.Now;
        int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
        int rows = (int)Math.Ceiling(daysInMonth / 7.0);

        int rowIndex = (Math.Clamp(current ? totalSignDay : totalSignDay + 1, 1, daysInMonth) - 1) / 7;
        double offset = rowIndex * (scrollViewer.ExtentHeight / rows);
        scrollViewer.ChangeView(null, offset, null);
    }

    [Command("ClaimSignInRewardCommand")]
    private async Task ClaimSignInRewardAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            return;
        }

        if (await signInService.ClaimSignInRewardAsync(userAndUid).ConfigureAwait(false))
        {
            await UpdateSignInInfoAsync(userAndUid, postSign: true).ConfigureAwait(false);
        }
    }

    [Command("ClaimResignInRewardCommand")]
    private async Task ClaimResignInRewardAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            return;
        }

        string content = userAndUid.IsOversea
            ? SH.FormatViewModelSignInReSignInDialogContentOversea(resignInfo?.Cost, resignInfo?.QualityCount)
            : SH.FormatViewModelSignInReSignInDialogContent(resignInfo?.CoinCost, resignInfo?.CoinCount);

        ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(SH.ViewModelSignInReSignInDialogTitle, content).ConfigureAwait(false);
        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        if (await signInService.ClaimResignInRewardAsync(userAndUid).ConfigureAwait(false))
        {
            await UpdateSignInInfoAsync(userAndUid, postResign: true).ConfigureAwait(false);
        }
    }
}