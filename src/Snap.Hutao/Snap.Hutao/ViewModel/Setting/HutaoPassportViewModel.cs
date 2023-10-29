// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Response;
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
    private readonly HomaPassportClient homaPassportClient;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    [Command("OpenRedeemWebsiteCommand")]
    private static async Task OpenRedeemWebsiteAsync()
    {
        await Launcher.LaunchUriAsync("https://homa.snapgenshin.com/redeem.html".ToUri());
    }

    private static void SaveUserNameAndPassword(string username, string password)
    {
        LocalSetting.Set(SettingKeys.PassportUserName, username);
        LocalSetting.Set(SettingKeys.PassportPassword, password);
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

            Response<string> response = await homaPassportClient.RegisterAsync(username, password, verifyCode).ConfigureAwait(false);

            if (response.IsOk())
            {
                SaveUserNameAndPassword(username, password);
                infoBarService.Information(response.Message);

                await taskContext.SwitchToMainThreadAsync();
                hutaoUserOptions.LoginSucceed(username, response.Data);
            }
        }
    }

    [Command("UnregisterCommand")]
    private async Task UnregisterAsync()
    {
        HutaoPassportUnregisterDialog dialog = await contentDialogFactory.CreateInstanceAsync<HutaoPassportUnregisterDialog>().ConfigureAwait(false);
        ValueResult<bool, (string UserName, string Password)> result = await dialog.GetInputAsync().ConfigureAwait(false);

        if (result.IsOk)
        {
            (string username, string password) = result.Value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return;
            }

            Response response = await homaPassportClient.UnregisterAsync(username, password).ConfigureAwait(false);

            if (response.IsOk())
            {
                infoBarService.Information(response.Message);

                await taskContext.SwitchToMainThreadAsync();
                hutaoUserOptions.LogoutOrUnregister();
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

            Response<string> response = await homaPassportClient.LoginAsync(username, password).ConfigureAwait(false);

            if (response.IsOk())
            {
                SaveUserNameAndPassword(username, password);
                infoBarService.Information(response.Message);

                await taskContext.SwitchToMainThreadAsync();
                hutaoUserOptions.LoginSucceed(username, response.Data);
            }
        }
    }

    [Command("LogoutCommand")]
    private void LogoutAsync()
    {
        hutaoUserOptions.LogoutOrUnregister();
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

            Response<string> response = await homaPassportClient.ResetPasswordAsync(username, password, verifyCode).ConfigureAwait(false);

            if (response.IsOk())
            {
                SaveUserNameAndPassword(username, password);
                infoBarService.Information(response.Message);

                await taskContext.SwitchToMainThreadAsync();
                hutaoUserOptions.LoginSucceed(username, response.Data);
            }
        }
    }
}