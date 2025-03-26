// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Sign;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Transient)]
internal sealed partial class SignInViewModel : Abstraction.ViewModelSlim, IRecipient<UserAndUidChangedMessage>
{
    private readonly WeakReference<ScrollViewer> weakAwardScrollViewer = new(default!);

    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;

    private bool updating;
    private int totalSignDay;
    private SignInRewardReSignInfo? resignInfo;

    [ObservableProperty]
    public partial IAdvancedCollectionView<AwardView>? Awards { get; set; }

    [ObservableProperty]
    public partial string? TotalSignInDaysHint { get; set; }

    [ObservableProperty]
    public partial string? CurrentUid { get; set; }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (updating)
        {
            return;
        }

        IsInitialized = false;
        if (message.UserAndUid is { } userAndUid)
        {
            UpdateSignInInfoAsync(userAndUid).SafeForget();
        }
        else
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
        }
    }

    public void AttachXamlElement(ScrollViewer scrollViewer)
    {
        weakAwardScrollViewer.SetTarget(scrollViewer);
    }

    protected override async Task LoadAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            await UpdateSignInInfoAsync(userAndUid).ConfigureAwait(false);
        }
        else
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
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
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                ISignInClient signInClient = scope.ServiceProvider
                    .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
                    .Create(userAndUid.IsOversea);

                Response<Reward> rewardResponse = await signInClient.GetRewardAsync(userAndUid.User).ConfigureAwait(false);
                if (!ResponseValidator.TryValidate(rewardResponse, serviceProvider, out reward))
                {
                    infoBarService.Error(SH.ServiceSignInRewardListRequestFailed);
                    return;
                }

                Response<SignInRewardInfo> infoResponse = await signInClient.GetInfoAsync(userAndUid).ConfigureAwait(false);
                if (!ResponseValidator.TryValidate(infoResponse, serviceProvider, out info))
                {
                    infoBarService.Error(SH.ServiceSignInInfoRequestFailed);
                    return;
                }

                Response<SignInRewardReSignInfo> resignInfoResponse = await signInClient.GetResignInfoAsync(userAndUid).ConfigureAwait(false);
                if (!ResponseValidator.TryValidate(resignInfoResponse, serviceProvider, out resignInfo))
                {
                    infoBarService.Error(SH.ServiceSignInInfoRequestFailed);
                    return;
                }
            }

            if (postSign)
            {
                int index = info.TotalSignDay - 1;
                Award award = reward.Awards[index];
                infoBarService.Success(SH.FormatServiceSignInSuccessRewardFormat(award.Name, award.Count));
            }
            else if (postResign)
            {
                int index = info.TotalSignDay - 1;
                Award award = reward.Awards[index];
                infoBarService.Success(SH.FormatServiceReSignInSuccessRewardFormat(award.Name, award.Count));
            }

            ImmutableArray<AwardView> avs = reward.Awards.Select(AwardView.From).ToImmutableArray();
            InitializeClaimedAwards(avs, info.TotalSignDay);

            this.totalSignDay = info.TotalSignDay;
            this.resignInfo = resignInfo;

            await taskContext.SwitchToMainThreadAsync();
            Awards = avs.AsAdvancedCollectionView();
            CurrentUid = userAndUid.Uid.ToString();
            TotalSignInDaysHint = SH.FormatViewModelSignInTotalSignInDaysHint(reward.Month, info.TotalSignDay);
            ScrollToCurrentOrNextAward(postSign || postResign);

            IsInitialized = true;
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
        if (!weakAwardScrollViewer.TryGetTarget(out ScrollViewer? scrollViewer))
        {
            return;
        }

        DateTime now = DateTime.Now;
        int days = DateTime.DaysInMonth(now.Year, now.Month);
        int targetIndex = current ? totalSignDay - 1 : totalSignDay;

        int row = targetIndex / 7;
        int rows = (int)Math.Ceiling(days / 7.0);

        ArgumentNullException.ThrowIfNull(weakAwardScrollViewer);
        double offset = row * scrollViewer.ExtentHeight / rows;
        scrollViewer.ChangeView(null, offset, null);
    }

    [Command("ClaimSignInRewardCommand")]
    private async Task ClaimSignInRewardAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISignInClient signInClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
                .Create(userAndUid.IsOversea);

            Response<SignInResult> resultResponse = await signInClient.SignAsync(userAndUid).ConfigureAwait(false);
            if (!ResponseValidator.TryValidateWithoutUINotification(resultResponse, out SignInResult? result))
            {
                string message = resultResponse.Message;

                if (resultResponse.ReturnCode is (int)KnownReturnCode.AlreadySignedIn)
                {
                    infoBarService.Success(message);
                    return;
                }

                if (string.IsNullOrEmpty(message))
                {
                    message = $"RiskCode: {result?.RiskCode}";
                }

                infoBarService.Error(SH.FormatServiceSignInClaimRewardFailedFormat(message));
                await FallbackToWebView2SignInAsync().ConfigureAwait(false);
                return;
            }

            await UpdateSignInInfoAsync(userAndUid, postSign: true).ConfigureAwait(false);
        }
    }

    [Command("ClaimResignInRewardCommand")]
    private async Task ClaimResignInRewardAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        string content = userAndUid.IsOversea
            ? SH.FormatViewModelSignInReSignInDialogContentOversea(resignInfo?.Cost, resignInfo?.QualityCount)
            : SH.FormatViewModelSignInReSignInDialogContent(resignInfo?.CoinCost, resignInfo?.CoinCount);

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(
                SH.ViewModelSignInReSignInDialogTitle,
                content)
            .ConfigureAwait(false);
        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISignInClient signInClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
                .Create(userAndUid.IsOversea);

            Response<SignInResult> resultResponse = await signInClient.ReSignAsync(userAndUid).ConfigureAwait(false);
            if (!ResponseValidator.TryValidateWithoutUINotification(resultResponse, out SignInResult? signInResult))
            {
                string message = resultResponse.Message;

                if (resultResponse.ReturnCode is (int)KnownReturnCode.ResignQuotaUsedUp or (int)KnownReturnCode.PleaseSignInFirst or (int)KnownReturnCode.NoAvailableResignDate)
                {
                    infoBarService.Error(message);
                    return;
                }

                if (resultResponse.ReturnCode is (int)KnownReturnCode.NotEnoughCoin)
                {
                    message = SH.ViewModelSignInReSignInNotEnoughCoinMessage;
                    infoBarService.Error(message);
                    return;
                }

                if (string.IsNullOrEmpty(message))
                {
                    message = $"RiskCode: {signInResult?.RiskCode}";
                }

                infoBarService.Error(SH.FormatServiceReSignInClaimRewardFailedFormat(message));
                await FallbackToWebView2SignInAsync().ConfigureAwait(false);
                return;
            }

            await UpdateSignInInfoAsync(userAndUid, postResign: true).ConfigureAwait(false);
        }
    }

    private async ValueTask FallbackToWebView2SignInAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        MiHoYoJSBridgeWebView2ContentProvider provider = new()
        {
            SourceProvider = new SignInJSBridgeUriSourceProvider(),
        };

        ShowWebView2WindowAction.Show(provider, currentXamlWindowReference.GetXamlRoot());
    }
}