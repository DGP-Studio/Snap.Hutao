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

[GeneratedConstructor]
[Service(ServiceLifetime.Singleton, typeof(ISignInService))]
internal sealed partial class SignInService : ISignInService
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    public async ValueTask<bool> ClaimSignInRewardAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISignInClient signInClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
                .Create(userAndUid.IsOversea);

            Response<SignInResult> resultResponse = await signInClient.SignAsync(userAndUid, token).ConfigureAwait(false);
            if (ResponseValidator.TryValidateWithoutUINotification(resultResponse, out SignInResult? result))
            {
                return true;
            }

            string message = resultResponse.Message;

            if (resultResponse.ReturnCode is (int)KnownReturnCode.AlreadySignedIn)
            {
                messenger.Send(InfoBarMessage.Success(message));
                return false;
            }

            if (string.IsNullOrEmpty(message))
            {
                message = $"RiskCode: {result?.RiskCode}";
            }

            messenger.Send(InfoBarMessage.Error(SH.FormatServiceSignInClaimRewardFailed(message)));
            await FallbackToWebView2SignInAsync().ConfigureAwait(false);
            return false;
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
            if (ResponseValidator.TryValidateWithoutUINotification(resultResponse, out SignInResult? signInResult))
            {
                return true;
            }

            string message = resultResponse.Message;

            if (resultResponse.ReturnCode is (int)KnownReturnCode.ResignQuotaUsedUp or (int)KnownReturnCode.PleaseSignInFirst or (int)KnownReturnCode.NoAvailableResignDate)
            {
                messenger.Send(InfoBarMessage.Error(message));
                return false;
            }

            if (resultResponse.ReturnCode is (int)KnownReturnCode.NotEnoughCoin)
            {
                message = SH.ViewModelSignInReSignInNotEnoughCoinMessage;
                messenger.Send(InfoBarMessage.Error(message));
                return false;
            }

            if (string.IsNullOrEmpty(message))
            {
                message = $"RiskCode: {signInResult?.RiskCode}";
            }

            messenger.Send(InfoBarMessage.Error(SH.FormatServiceReSignInClaimRewardFailed(message)));
            await FallbackToWebView2SignInAsync().ConfigureAwait(false);
            return false;
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