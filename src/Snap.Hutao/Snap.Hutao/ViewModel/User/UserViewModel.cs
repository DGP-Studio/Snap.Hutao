// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.View.Page;
using Snap.Hutao.Web.Hoyolab;
using System.Collections.ObjectModel;
using System.Text;

namespace Snap.Hutao.ViewModel.User;

/// <summary>
/// 用户视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton)]
internal sealed class UserViewModel : ObservableObject
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IInfoBarService infoBarService;
    private readonly Core.HutaoOptions hutaoOptions;

    private User? selectedUser;
    private ObservableCollection<User>? users;

    /// <summary>
    /// 构造一个新的用户视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="userService">用户服务</param>
    /// <param name="infoBarService">信息条服务</param>
    public UserViewModel(IServiceProvider serviceProvider)
    {
        hutaoOptions = serviceProvider.GetRequiredService<Core.HutaoOptions>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        userService = serviceProvider.GetRequiredService<IUserService>();
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        this.serviceProvider = serviceProvider;

        OpenUICommand = new AsyncRelayCommand(OpenUIAsync);
        AddUserCommand = new AsyncRelayCommand(AddUserAsync);
        AddOverseaUserCommand = new AsyncRelayCommand(AddOverseaUserAsync);
        LoginMihoyoUserCommand = new RelayCommand(LoginMihoyoUser);
        LoginHoyoverseUserCommand = new RelayCommand(LoginHoyoverseUser);
        RemoveUserCommand = new AsyncRelayCommand<User>(RemoveUserAsync);
        CopyCookieCommand = new RelayCommand<User>(CopyCookie);
        RefreshCookieTokenCommand = new AsyncRelayCommand(RefreshCookieTokenAsync);
    }

    /// <summary>
    /// 当前选择的用户信息
    /// </summary>
    public User? SelectedUser
    {
        get => selectedUser;
        set
        {
            if (SetProperty(ref selectedUser, value))
            {
                userService.Current = value;

                if (value != null)
                {
                    value.SelectedUserGameRole = value.UserGameRoles.FirstOrFirstOrDefault(role => role.IsChosen);
                }
            }
        }
    }

    /// <summary>
    /// 用户信息集合
    /// </summary>
    public ObservableCollection<User>? Users { get => users; set => SetProperty(ref users, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 添加用户命令
    /// </summary>
    public ICommand AddUserCommand { get; }

    /// <summary>
    /// 添加国际服用户命令
    /// </summary>
    public ICommand AddOverseaUserCommand { get; }

    /// <summary>
    /// 登录米游社命令
    /// </summary>
    public ICommand LoginMihoyoUserCommand { get; }

    /// <summary>
    /// 登录米游社命令
    /// </summary>
    public ICommand LoginHoyoverseUserCommand { get; }

    /// <summary>
    /// 移除用户命令
    /// </summary>
    public ICommand RemoveUserCommand { get; }

    /// <summary>
    /// 复制Cookie命令
    /// </summary>
    public ICommand CopyCookieCommand { get; }

    /// <summary>
    /// 刷新 CookieToken 命令
    /// </summary>
    public ICommand RefreshCookieTokenCommand { get; }

    /// <summary>
    /// 处理用户操作结果
    /// </summary>
    /// <param name="optionResult">操作结果</param>
    /// <param name="uid">uid</param>
    /// <returns>任务</returns>
    public async Task HandleUserOptionResultAsync(UserOptionResult optionResult, string uid)
    {
        switch (optionResult)
        {
            case UserOptionResult.Added:
                if (Users!.Count == 1)
                {
                    await taskContext.SwitchToMainThreadAsync();
                    SelectedUser = Users.Single();
                }

                infoBarService.Success(string.Format(SH.ViewModelUserAdded, uid));
                break;
            case UserOptionResult.Incomplete:
                infoBarService.Information(SH.ViewModelUserIncomplete);
                break;
            case UserOptionResult.Invalid:
                infoBarService.Information(SH.ViewModelUserInvalid);
                break;
            case UserOptionResult.Updated:
                infoBarService.Success(string.Format(SH.ViewModelUserUpdated, uid));
                break;
            default:
                throw Must.NeverHappen();
        }
    }

    private async Task OpenUIAsync()
    {
        try
        {
            Users = await userService.GetUserCollectionAsync().ConfigureAwait(true);
            SelectedUser = userService.Current;
        }
        catch (UserdataCorruptedException ex)
        {
            infoBarService.Error(ex);
        }
    }

    private Task AddUserAsync()
    {
        return AddUserCoreAsync(false);
    }

    private Task AddOverseaUserAsync()
    {
        return AddUserCoreAsync(true);
    }

    private async Task AddUserCoreAsync(bool isOversea)
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();

        // Get cookie from user input
        UserDialog dialog = serviceProvider.CreateInstance<UserDialog>();
        ValueResult<bool, string> result = await dialog.GetInputCookieAsync().ConfigureAwait(false);

        // User confirms the input
        if (result.IsOk)
        {
            Cookie cookie = Cookie.Parse(result.Value);

            (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(cookie, isOversea).ConfigureAwait(false);

            await HandleUserOptionResultAsync(optionResult, uid).ConfigureAwait(false);
        }
    }

    private void LoginMihoyoUser()
    {
        if (hutaoOptions.IsWebView2Supported)
        {
            serviceProvider
                .GetRequiredService<INavigationService>()
                .Navigate<LoginMihoyoUserPage>(INavigationAwaiter.Default);
        }
        else
        {
            infoBarService.Warning(SH.CoreWebView2HelperVersionUndetected);
        }
    }

    /// <summary>
    /// 打开浏览器登录 hoyolab 以获取 cookie
    /// </summary>
    private void LoginHoyoverseUser()
    {
        if (hutaoOptions.IsWebView2Supported)
        {
            serviceProvider
                .GetRequiredService<INavigationService>()
                .Navigate<LoginHoyoverseUserPage>(INavigationAwaiter.Default);
        }
        else
        {
            infoBarService.Warning(SH.CoreWebView2HelperVersionUndetected);
        }
    }

    private async Task RemoveUserAsync(User? user)
    {
        if (user != null)
        {
            try
            {
                await userService.RemoveUserAsync(user!).ConfigureAwait(false);
                infoBarService.Success(string.Format(SH.ViewModelUserRemoved, user.UserInfo?.Nickname));
            }
            catch (UserdataCorruptedException ex)
            {
                infoBarService.Error(ex);
            }
        }
    }

    private void CopyCookie(User? user)
    {
        try
        {
            string cookieString = new StringBuilder()
                .Append(user!.SToken)
                .AppendIf(user.SToken != null, ';')
                .Append(user.LToken)
                .AppendIf(user.LToken != null, ';')
                .Append(user.CookieToken)
                .ToString();

            Clipboard.SetText(cookieString);
            infoBarService.Success(string.Format(SH.ViewModelUserCookieCopied, user.UserInfo!.Nickname));
        }
        catch (Exception e)
        {
            infoBarService.Error(e);
        }
    }

    private async Task RefreshCookieTokenAsync()
    {
        if (SelectedUser != null)
        {
            if (await userService.RefreshCookieTokenAsync(SelectedUser).ConfigureAwait(false))
            {
                infoBarService.Success(SH.ViewUserRefreshCookieTokenSuccess);
            }
            else
            {
                infoBarService.Warning(SH.ViewUserRefreshCookieTokenWarning);
            }
        }
    }
}