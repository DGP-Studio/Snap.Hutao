// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 胡桃通行证视图模型
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class HutaoPassportViewModel : Abstraction.ViewModel
{
    private readonly HomaPassportClient homaPassportClient;
    private readonly INavigationService navigationService;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    private string? userName;
    private string? password;
    private string? verifyCode;

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get => userName; set => SetProperty(ref userName, value); }

    /// <summary>
    /// 密码
    /// </summary>
    public string? Password { get => password; set => SetProperty(ref password, value); }

    /// <summary>
    /// 验证码
    /// </summary>
    public string? VerifyCode { get => verifyCode; set => SetProperty(ref verifyCode, value); }

    [Command("RegisterVerifyCommand")]
    private Task RegisterVerifyAsync()
    {
        return VerifyAsync(false).AsTask();
    }

    [Command("RegisterCommand")]
    private async Task RegisterAsync()
    {
        if (UserName is null || Password is null || VerifyCode is null)
        {
            return;
        }

        Response<string> response = await homaPassportClient.RegisterAsync(UserName, Password, VerifyCode).ConfigureAwait(false);

        if (response.IsOk())
        {
            SaveUserNameAndPassword();
            infoBarService.Information(response.Message);

            await taskContext.SwitchToMainThreadAsync();
            hutaoUserOptions.LoginSucceed(UserName, response.Data);

            await navigationService
                .NavigateAsync<View.Page.SettingPage>(INavigationAwaiter.Default, true)
                .ConfigureAwait(false);
        }
    }

    [Command("ResetPasswordVerifyCommand")]
    private Task ResetPasswordVerifyAsync()
    {
        return VerifyAsync(true).AsTask();
    }

    [Command("ResetPasswordCommand")]
    private async Task ResetPasswordAsync()
    {
        if (UserName is null || Password is null || VerifyCode is null)
        {
            return;
        }

        Response<string> response = await homaPassportClient.ResetPasswordAsync(UserName, Password, VerifyCode).ConfigureAwait(false);

        if (response.IsOk())
        {
            SaveUserNameAndPassword();
            infoBarService.Information(response.Message);

            await taskContext.SwitchToMainThreadAsync();
            hutaoUserOptions.LoginSucceed(UserName, response.Data);

            await navigationService
                .NavigateAsync<View.Page.SettingPage>(INavigationAwaiter.Default, true)
                .ConfigureAwait(false);
        }
    }

    [Command("LoginCommand")]
    private async Task LoginAsync()
    {
        if (UserName is null || Password is null)
        {
            return;
        }

        Response<string> response = await homaPassportClient.LoginAsync(UserName, Password).ConfigureAwait(false);

        if (response.IsOk())
        {
            SaveUserNameAndPassword();
            infoBarService.Information(response.Message);

            await taskContext.SwitchToMainThreadAsync();
            hutaoUserOptions.LoginSucceed(UserName, response.Data);

            await navigationService
                .NavigateAsync<View.Page.SettingPage>(INavigationAwaiter.Default, true)
                .ConfigureAwait(false);
        }
    }

    private async ValueTask VerifyAsync(bool isResetPassword)
    {
        if (string.IsNullOrEmpty(UserName))
        {
            return;
        }

        if (!UserName.IsEmail())
        {
            infoBarService.Warning(SH.ViewModelHutaoPassportEmailNotValidHint);
        }

        Response response = await homaPassportClient.VerifyAsync(UserName, isResetPassword).ConfigureAwait(false);
        infoBarService.Information(response.Message);
    }

    private void SaveUserNameAndPassword()
    {
        if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
        {
            LocalSetting.Set(SettingKeys.PassportUserName, UserName);
            LocalSetting.Set(SettingKeys.PassportPassword, Password);
        }
    }
}