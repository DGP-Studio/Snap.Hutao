// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.SignIn;

[ConstructorGenerated]
[Service(ServiceLifetime.Singleton, typeof(ISignInService))]
internal sealed partial class SignInService : ISignInService
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    public async ValueTask<bool> ClaimSignInRewardAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISignInClient signInClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
                .Create(userAndUid.IsOversea);

            Response<SignInResult> resultResponse = await signInClient.SignAsync(userAndUid, token).ConfigureAwait(false);
            if (!ResponseValidator.TryValidateWithoutUINotification(resultResponse, out SignInResult? result))
            {
                string message = resultResponse.Message;

                if (resultResponse.ReturnCode is (int)KnownReturnCode.AlreadySignedIn)
                {
                    infoBarService.Success(message);
                    return false;
                }

                if (string.IsNullOrEmpty(message))
                {
                    message = $"RiskCode: {result?.RiskCode}";
                }

                infoBarService.Error(SH.FormatServiceSignInClaimRewardFailed(message));
                await FallbackToWebView2SignInAsync().ConfigureAwait(false);
                return false;
            }

            return true;
        }
    }

    public async ValueTask<bool> ClaimResignInRewardAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISignInClient signInClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
                .Create(userAndUid.IsOversea);

            Response<SignInResult> resultResponse = await signInClient.ReSignAsync(userAndUid, token).ConfigureAwait(false);
            if (!ResponseValidator.TryValidateWithoutUINotification(resultResponse, out SignInResult? signInResult))
            {
                string message = resultResponse.Message;

                if (resultResponse.ReturnCode is (int)KnownReturnCode.ResignQuotaUsedUp or (int)KnownReturnCode.PleaseSignInFirst or (int)KnownReturnCode.NoAvailableResignDate)
                {
                    infoBarService.Error(message);
                    return false;
                }

                if (resultResponse.ReturnCode is (int)KnownReturnCode.NotEnoughCoin)
                {
                    message = SH.ViewModelSignInReSignInNotEnoughCoinMessage;
                    infoBarService.Error(message);
                    return false;
                }

                if (string.IsNullOrEmpty(message))
                {
                    message = $"RiskCode: {signInResult?.RiskCode}";
                }

                infoBarService.Error(SH.FormatServiceReSignInClaimRewardFailed(message));
                await FallbackToWebView2SignInAsync().ConfigureAwait(false);
                return false;
            }

            return true;
        }
    }

    private async ValueTask FallbackToWebView2SignInAsync()
    {
        await taskContext.SwitchToMainThreadAsync();

        if (currentXamlWindowReference.GetXamlRoot() is not { } xamlRoot)
        {
            return;
        }

        MiHoYoJSBridgeWebView2ContentProvider provider = new()
        {
            SourceProvider = new SignInJSBridgeUriSourceProvider(),
        };

        ShowWebView2WindowAction.Show(provider, xamlRoot);
    }
}