// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.Web;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Windows.System;

namespace Snap.Hutao.ViewModel.Setting;

/// <summary>
/// 胡桃通行证视图模型
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class HutaoPassportViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    [Command("OpenRedeemWebsiteCommand")]
    private static async Task OpenRedeemWebsiteAsync()
    {
        await Launcher.LaunchUriAsync(HutaoEndpoints.Website("redeem.html").ToUri());
    }

    [Command("RegisterCommand")]
    private async Task RegisterAsync()
    {
        HutaoPassportRegisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportRegisterDialog>().ConfigureAwait(false);
        ValueResult<bool, (string UserName, string Password, string VerifyCode)> result = await dialog.GetInputAsync().ConfigureAwait(false);

        if (result.IsOk)
        {
            (string username, string password, string verifyCode) = result.Value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
            {
                return;
            }

            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();

                HutaoResponse<string> response = await hutaoPassportClient.RegisterAsync(username, password, verifyCode).ConfigureAwait(false);

                if (response.IsOk())
                {
                    infoBarService.Information(response.GetLocalizationMessageOrMessage());
                    await hutaoUserOptions.PostLoginSucceedAsync(hutaoPassportClient, taskContext, username, password, response.Data).ConfigureAwait(false);
                }
            }
        }
    }

    [Command("UnregisterCommand")]
    private async Task UnregisterAsync()
    {
        HutaoPassportUnregisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportUnregisterDialog>().ConfigureAwait(false);
        ValueResult<bool, (string UserName, string Password, string VerifyCode)> result = await dialog.GetInputAsync().ConfigureAwait(false);

        if (result.IsOk)
        {
            (string username, string password, string verifyCode) = result.Value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
            {
                return;
            }

            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();

                HutaoResponse response = await hutaoPassportClient.UnregisterAsync(username, password, verifyCode).ConfigureAwait(false);

                if (response.IsOk())
                {
                    infoBarService.Information(response.GetLocalizationMessageOrMessage());

                    await taskContext.SwitchToMainThreadAsync();
                    hutaoUserOptions.PostLogoutOrUnregister();
                }
            }
        }
    }

    [Command("LoginCommand")]
    private async Task LoginAsync()
    {
        HutaoPassportLoginDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportLoginDialog>().ConfigureAwait(false);
        ValueResult<bool, (string UserName, string Password)> result = await dialog.GetInputAsync().ConfigureAwait(false);

        if (result.IsOk)
        {
            (string username, string password) = result.Value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return;
            }

            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();

                HutaoResponse<string> response = await hutaoPassportClient.LoginAsync(username, password).ConfigureAwait(false);

                if (response.IsOk())
                {
                    infoBarService.Information(response.GetLocalizationMessageOrMessage());
                    await hutaoUserOptions.PostLoginSucceedAsync(hutaoPassportClient, taskContext, username, password, response.Data).ConfigureAwait(false);
                }
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

        if (result.IsOk)
        {
            (string username, string password, string verifyCode) = result.Value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(verifyCode))
            {
                return;
            }

            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();

                HutaoResponse<string> response = await hutaoPassportClient.ResetPasswordAsync(username, password, verifyCode).ConfigureAwait(false);

                if (response.IsOk())
                {
                    infoBarService.Information(response.GetLocalizationMessageOrMessage());
                    await hutaoUserOptions.PostLoginSucceedAsync(hutaoPassportClient, taskContext, username, password, response.Data).ConfigureAwait(false);
                }
            }
        }
    }
}