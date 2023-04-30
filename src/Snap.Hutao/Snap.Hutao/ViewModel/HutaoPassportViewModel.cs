// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 胡桃通行证视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal sealed class HutaoPassportViewModel : Abstraction.ViewModel
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly HomaPassportClient homaPassportClient;

    private string? userName;
    private string? password;
    private string? verifyCode;

    /// <summary>
    /// 构造一个新的胡桃通行证视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public HutaoPassportViewModel(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        homaPassportClient = serviceProvider.GetRequiredService<HomaPassportClient>();
        this.serviceProvider = serviceProvider;

        RegisterVerifyCommand = new AsyncRelayCommand(RegisterVerifyAsync);
        RegisterCommand = new AsyncRelayCommand(RegisterAsync);
        ResetPasswordVerifyCommand = new AsyncRelayCommand(ResetPasswordVerifyAsync);
        ResetPasswordCommand = new AsyncRelayCommand(ResetPasswordAsync);
        LoginCommand = new AsyncRelayCommand(LoginAsync);
    }

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

    /// <summary>
    /// 注册验证命令
    /// </summary>
    public ICommand RegisterVerifyCommand { get; }

    /// <summary>
    /// 注册命令
    /// </summary>
    public ICommand RegisterCommand { get; }

    /// <summary>
    /// 注册验证命令
    /// </summary>
    public ICommand ResetPasswordVerifyCommand { get; }

    /// <summary>
    /// 注册命令
    /// </summary>
    public ICommand ResetPasswordCommand { get; }

    /// <summary>
    /// 登录命令
    /// </summary>
    public ICommand LoginCommand { get; }

    /// <inheritdoc/>
    protected override Task OpenUIAsync()
    {
        return Task.CompletedTask;
    }

    private Task RegisterVerifyAsync()
    {
        return VerifyAsync(false);
    }

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

    private Task ResetPasswordVerifyAsync()
    {
        return VerifyAsync(true);
    }

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