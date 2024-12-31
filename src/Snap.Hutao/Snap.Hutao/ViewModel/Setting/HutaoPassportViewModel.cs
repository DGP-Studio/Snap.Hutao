// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.Web.Endpoint.Hutao;
using Windows.System;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class HutaoPassportViewModel : Abstraction.ViewModel
{
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly INavigationService navigationService;

    public partial HutaoUserOptions HutaoUserOptions { get; }

    [Command("OpenRedeemWebsiteCommand")]
    private async Task OpenRedeemWebsiteAsync()
    {
        await Launcher.LaunchUriAsync(hutaoEndpointsFactory.Create().HomaWebsite("redeem.html").ToUri());
    }

    [Command("OpenTestPageCommand")]
    private async Task OpenTestPageAsync()
    {
        await navigationService.NavigateAsync<TestPage>(INavigationCompletionSource.Default).ConfigureAwait(false);
    }

    [Command("RegisterCommand")]
    private async Task RegisterAsync()
    {
        HutaoPassportRegisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportRegisterDialog>().ConfigureAwait(false);
        ValueResult<bool, (string UserName, string Password, string VerifyCode)> result = await dialog.GetInputAsync().ConfigureAwait(false);

        if (!result.IsOk)
        {
            return;
        }

        (string username, string password, string verifyCode) = result.Value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
        {
            return;
        }

        await HutaoUserOptions.RegisterAsync(username, password, verifyCode).ConfigureAwait(false);
    }

    [Command("UnregisterCommand")]
    private async Task UnregisterAsync()
    {
        string? userName = await HutaoUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        HutaoPassportUnregisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportUnregisterDialog>().ConfigureAwait(false);
        ValueResult<bool, (string UserName, string Password, string VerifyCode)> result = await dialog.GetInputAsync(userName).ConfigureAwait(false);

        if (!result.IsOk)
        {
            return;
        }

        (string username, string password, string verifyCode) = result.Value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
        {
            return;
        }

        await HutaoUserOptions.UnregisterAsync(username, password, verifyCode).ConfigureAwait(false);
    }

    [Command("LoginCommand")]
    private async Task LoginAsync()
    {
        HutaoPassportLoginDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportLoginDialog>().ConfigureAwait(false);
        ValueResult<bool, (string UserName, string Password)> result = await dialog.GetInputAsync().ConfigureAwait(false);

        if (!result.IsOk)
        {
            return;
        }

        (string username, string password) = result.Value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return;
        }

        await HutaoUserOptions.LoginAsync(username, password, false).ConfigureAwait(false);
    }

    [Command("LogoutCommand")]
    private async Task LogoutAsync()
    {
        await HutaoUserOptions.LogoutAsync().ConfigureAwait(false);
    }

    [Command("ResetUsernameCommand")]
    private async Task ResetUsernameAsync()
    {
        string? userName = await HutaoUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        HutaoPassportResetUsernameDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportResetUsernameDialog>().ConfigureAwait(false);
        ValueResult<bool, (string UserName, string NewUserName, string VerifyCode, string NewVerifyCode)> result = await dialog.GetInputAsync(userName).ConfigureAwait(false);

        if (!result.IsOk)
        {
            return;
        }

        (string username, string newUserName, string verifyCode, string newVerifyCode) = result.Value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newUserName) || string.IsNullOrEmpty(verifyCode) || string.IsNullOrEmpty(newVerifyCode))
        {
            return;
        }

        await HutaoUserOptions.ResetUserNameAsync(username, newUserName, verifyCode, newVerifyCode).ConfigureAwait(false);
    }

    [Command("ResetPasswordCommand")]
    private async Task ResetPasswordAsync()
    {
        string? userName = await HutaoUserOptions.GetActualUserNameAsync().ConfigureAwait(false);

        HutaoPassportResetPasswordDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportResetPasswordDialog>().ConfigureAwait(false);
        ValueResult<bool, (string UserName, string Password, string VerifyCode)> result = await dialog.GetInputAsync(userName).ConfigureAwait(false);

        if (!result.IsOk)
        {
            return;
        }

        (string username, string password, string verifyCode) = result.Value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
        {
            return;
        }

        await HutaoUserOptions.ResetPasswordAsync(username, password, verifyCode).ConfigureAwait(false);
    }
}