// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly HomaPassportClient homaPassportClient;

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

    /// <inheritdoc/>
    protected override Task OpenUIAsync()
    {
        return Task.CompletedTask;
    }

    [Command("RegisterVerifyCommand")]
    private Task RegisterVerifyAsync()
    {
        return VerifyAsync(false);
    }

    [Command("RegisterCommand")]
    private async Task RegisterAsync()
    {
        if (UserName == null || Password == null || VerifyCode == null)
        {
            return;
        }

        Response<string> response = await homaPassportClient.RegisterAsync(UserName, Password, VerifyCode).ConfigureAwait(false);

        if (response.IsOk())
        {
            SaveUserNameAndPassword();
            serviceProvider.GetRequiredService<IInfoBarService>().Information(response.Message);

            await taskContext.SwitchToMainThreadAsync();
            serviceProvider.GetRequiredService<HutaoUserOptions>().LoginSucceed(UserName, response.Data);

            await serviceProvider
                .GetRequiredService<INavigationService>()
                .NavigateAsync<View.Page.SettingPage>(INavigationAwaiter.Default, true)
                .ConfigureAwait(false);
        }
    }

    [Command("ResetPasswordVerifyCommand")]
    private Task ResetPasswordVerifyAsync()
    {
        return VerifyAsync(true);
    }

    [Command("ResetPasswordCommand")]
    private async Task ResetPasswordAsync()
    {
        if (UserName == null || Password == null || VerifyCode == null)
        {
            return;
        }

        Response<string> response = await homaPassportClient.ResetPasswordAsync(UserName, Password, VerifyCode).ConfigureAwait(false);

        if (response.IsOk())
        {
            SaveUserNameAndPassword();
            serviceProvider.GetRequiredService<IInfoBarService>().Information(response.Message);

            await taskContext.SwitchToMainThreadAsync();
            serviceProvider.GetRequiredService<HutaoUserOptions>().LoginSucceed(UserName, response.Data);

            await serviceProvider
                .GetRequiredService<INavigationService>()
                .NavigateAsync<View.Page.SettingPage>(INavigationAwaiter.Default, true)
                .ConfigureAwait(false);
        }
    }

    [Command("LoginCommand")]
    private async Task LoginAsync()
    {
        if (UserName == null || Password == null)
        {
            return;
        }

        Response<string> response = await homaPassportClient.LoginAsync(UserName, Password).ConfigureAwait(false);

        if (response.IsOk())
        {
            SaveUserNameAndPassword();
            serviceProvider.GetRequiredService<IInfoBarService>().Information(response.Message);

            await taskContext.SwitchToMainThreadAsync();
            serviceProvider.GetRequiredService<HutaoUserOptions>().LoginSucceed(UserName, response.Data);

            await serviceProvider
                .GetRequiredService<INavigationService>()
                .NavigateAsync<View.Page.SettingPage>(INavigationAwaiter.Default, true)
                .ConfigureAwait(false);
        }
    }

    private async Task VerifyAsync(bool isResetPassword)
    {
        if (UserName == null)
        {
            return;
        }

        Response response = await homaPassportClient.VerifyAsync(UserName, isResetPassword).ConfigureAwait(false);
        serviceProvider.GetRequiredService<IInfoBarService>().Information(response.Message);
    }

    private void SaveUserNameAndPassword()
    {
        if (UserName != null && Password != null)
        {
            LocalSetting.Set(SettingKeys.PassportUserName, UserName);
            LocalSetting.Set(SettingKeys.PassportPassword, Password);
        }
    }
}