// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;
using Windows.System;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class HutaoPassportViewModel : Abstraction.ViewModel
{
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly INavigationService navigationService;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    public HutaoUserOptions User { get => hutaoUserOptions; }

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

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
            HutaoResponse<string> response = await hutaoPassportClient.RegisterAsync(username, password, verifyCode).ConfigureAwait(false);

            if (ResponseValidator.TryValidate(response, infoBarService, out string? token))
            {
                infoBarService.Information(response.GetLocalizationMessageOrMessage());
                await hutaoUserOptions.PostLoginSucceedAsync(scope.ServiceProvider, username, password, token).ConfigureAwait(false);
            }
        }
    }

    [Command("UnregisterCommand")]
    private async Task UnregisterAsync()
    {
        HutaoPassportUnregisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportUnregisterDialog>().ConfigureAwait(false);
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

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
            HutaoResponse response = await hutaoPassportClient.UnregisterAsync(username, password, verifyCode).ConfigureAwait(false);

            if (ResponseValidator.TryValidate(response, infoBarService))
            {
                infoBarService.Information(response.GetLocalizationMessageOrMessage());

                await taskContext.SwitchToMainThreadAsync();
                hutaoUserOptions.PostLogoutOrUnregister();
            }
        }
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

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
            HutaoResponse<string> response = await hutaoPassportClient.LoginAsync(username, password).ConfigureAwait(false);

            if (ResponseValidator.TryValidate(response, infoBarService, out string? token))
            {
                infoBarService.Information(response.GetLocalizationMessageOrMessage());
                await hutaoUserOptions.PostLoginSucceedAsync(scope.ServiceProvider, username, password, token).ConfigureAwait(false);
            }
        }
    }

    [Command("LogoutCommand")]
    private void LogoutAsync()
    {
        hutaoUserOptions.PostLogoutOrUnregister();
    }

    [Command("ResetPasswordCommand")]
    private async Task ResetPasswordAsync()
    {
        HutaoPassportResetPasswordDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportResetPasswordDialog>().ConfigureAwait(false);
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

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
            HutaoResponse<string> response = await hutaoPassportClient.ResetPasswordAsync(username, password, verifyCode).ConfigureAwait(false);

            if (ResponseValidator.TryValidate(response, infoBarService, out string? token))
            {
                infoBarService.Information(response.GetLocalizationMessageOrMessage());
                await hutaoUserOptions.PostLoginSucceedAsync(scope.ServiceProvider, username, password, token).ConfigureAwait(false);
            }
        }
    }
}