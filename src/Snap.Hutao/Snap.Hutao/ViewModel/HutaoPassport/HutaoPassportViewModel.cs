// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;

namespace Snap.Hutao.ViewModel.HutaoPassport;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class HutaoPassportViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly INavigationService navigationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial HutaoPassportViewModel(IServiceProvider serviceProvider);

    public partial HutaoUserOptions HutaoUserOptions { get; }

    [Command("OpenTestPageCommand")]
    private async Task OpenTestPageAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Navigate to TestPage", "HutaoPassportViewModel.Command"));
        await navigationService.NavigateAsync<TestPage>(NavigationExtraData.Default).ConfigureAwait(false);
    }

    [Command("RegisterCommand")]
    private async Task RegisterAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Register", "HutaoPassportViewModel.Command"));

        HutaoPassportRegisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportRegisterDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync().ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? password, string? verifyCode) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
        {
            return;
        }

        if (password.Length < 8)
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewModelHutaoPassportPasswordTooShortHint));
            return;
        }

        await HutaoUserOptions.RegisterAsync(username, password, verifyCode).ConfigureAwait(false);
    }

    [Command("UnregisterCommand")]
    private async Task UnregisterAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Unregister", "HutaoPassportViewModel.Command"));

        string? userName = await HutaoUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        HutaoPassportUnregisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportUnregisterDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync(userName).ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? password, string? verifyCode) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
        {
            return;
        }

        await HutaoUserOptions.UnregisterAsync(username, password, verifyCode).ConfigureAwait(false);
    }

    [Command("LoginCommand")]
    private async Task LoginAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Login", "HutaoPassportViewModel.Command"));

        HutaoPassportLoginDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportLoginDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync().ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? password) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return;
        }

        await HutaoUserOptions.LoginAsync(username, password, false).ConfigureAwait(false);
    }

    [Command("LogoutCommand")]
    private async Task LogoutAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Logout", "HutaoPassportViewModel.Command"));
        await HutaoUserOptions.LogoutAsync().ConfigureAwait(false);
    }

    [Command("ResetUsernameCommand")]
    private async Task ResetUsernameAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset email", "HutaoPassportViewModel.Command"));
        string? userName = await HutaoUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        HutaoPassportResetUsernameDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportResetUsernameDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync(userName).ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? newUserName, string? verifyCode, string? newVerifyCode) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newUserName) || string.IsNullOrEmpty(verifyCode) || string.IsNullOrEmpty(newVerifyCode))
        {
            return;
        }

        await HutaoUserOptions.ResetUserNameAsync(username, newUserName, verifyCode, newVerifyCode).ConfigureAwait(false);
    }

    [Command("ResetPasswordCommand")]
    private async Task ResetPasswordAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset password", "HutaoPassportViewModel.Command"));
        string? userName = await HutaoUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        HutaoPassportResetPasswordDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportResetPasswordDialog>(serviceProvider).ConfigureAwait(false);

        if (await dialog.GetInputAsync(userName).ConfigureAwait(false) is not (true, var result))
        {
            return;
        }

        (string? username, string? password, string? verifyCode) = result;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
        {
            return;
        }

        if (password.Length < 8)
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewModelHutaoPassportPasswordTooShortHint));
            return;
        }

        await HutaoUserOptions.ResetPasswordAsync(username, password, verifyCode).ConfigureAwait(false);
    }

    [Command("UseRedeemCodeCommand")]
    private async Task UseRedeemCodeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Use redeem code", "HutaoPassportViewModel.Command"));
        HutaoPassportUseRedeemCodeDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportUseRedeemCodeDialog>(serviceProvider).ConfigureAwait(false);
        if (await dialog.GetInputAsync().ConfigureAwait(false) is not (true, { Length: > 0 } redeemCode))
        {
            return;
        }

        await HutaoUserOptions.UseRedeemCodeAsync(redeemCode).ConfigureAwait(false);
    }

    [Command("RefreshUserInfoCommand")]
    private async Task RefreshUserInfoAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh user info", "HutaoPassportViewModel.Command"));
        await HutaoUserOptions.RefreshUserInfoAsync().ConfigureAwait(false);
    }
}