// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
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
[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class UserViewModel : ObservableObject
{
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly Core.HutaoOptions hutaoOptions;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;

    private User? selectedUser;
    private ObservableCollection<User>? users;

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

    [Command("OpenUICommand")]
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

    [Command("AddUserCommand")]
    private Task AddUserAsync()
    {
        return AddUserCoreAsync(false);
    }

    [Command("AddOverseaUserCommand")]
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

    [Command("LoginMihoyoUserCommand")]
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

    [Command("LoginHoyoverseUserCommand")]
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

    [Command("RemoveUserCommand")]
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

    [Command("CopyCookieCommand")]
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

    [Command("RefreshCookieTokenCommand")]
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