// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.SignIn;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.View.Page;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Account;
using Snap.Hutao.Web.Response;
using System.Collections.ObjectModel;
using System.Text;
using Windows.System;

namespace Snap.Hutao.ViewModel.User;

/// <summary>
/// 用户视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class UserViewModel : ObservableObject
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IDocumentationProvider documentationProvider;
    private readonly INavigationService navigationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ISignInService signInService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly SessionAppClient sessionAppClient;

    private User? selectedUser;
    private ObservableCollection<User>? users;

    /// <summary>
    /// 当前选择的用户信息
    /// </summary>
    public User? SelectedUser
    {
        get => selectedUser ??= userService.Current;
        set
        {
            if (SetProperty(ref selectedUser, value))
            {
                userService.Current = value;

                if (value is not null)
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
    internal async ValueTask HandleUserOptionResultAsync(UserOptionResult optionResult, string uid)
    {
        switch (optionResult)
        {
            case UserOptionResult.Added:
                ArgumentNullException.ThrowIfNull(Users);
                if (Users.Count == 1)
                {
                    await taskContext.SwitchToMainThreadAsync();
                    SelectedUser = Users.Single();
                }

                infoBarService.Success(SH.FormatViewModelUserAdded(uid));
                break;
            case UserOptionResult.Incomplete:
                infoBarService.Information(SH.ViewModelUserIncomplete);
                break;
            case UserOptionResult.Invalid:
                infoBarService.Information(SH.ViewModelUserInvalid);
                break;
            case UserOptionResult.Updated:
                infoBarService.Success(SH.FormatViewModelUserUpdated(uid));
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
        return AddUserCoreAsync(false).AsTask();
    }

    [Command("AddOverseaUserCommand")]
    private Task AddOverseaUserAsync()
    {
        return AddUserCoreAsync(true).AsTask();
    }

    private async ValueTask AddUserCoreAsync(bool isOversea)
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();

        // Get cookie from user input
        UserDialog dialog = await contentDialogFactory.CreateInstanceAsync<UserDialog>().ConfigureAwait(false);
        ValueResult<bool, string> result = await dialog.GetInputCookieAsync().ConfigureAwait(false);

        // User confirms the input
        if (result.TryGetValue(out string rawCookie))
        {
            Cookie cookie = Cookie.Parse(rawCookie);

            (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(cookie, isOversea).ConfigureAwait(false);

            await HandleUserOptionResultAsync(optionResult, uid).ConfigureAwait(false);
        }
    }

    [Command("LoginMihoyoUserCommand")]
    private void LoginMihoyoUser()
    {
        if (runtimeOptions.IsWebView2Supported)
        {
            navigationService.Navigate<LoginMihoyoUserPage>(INavigationAwaiter.Default);
        }
        else
        {
            infoBarService.Warning(SH.CoreWebView2HelperVersionUndetected);
        }
    }

    [Command("LoginHoyoverseUserCommand")]
    private void LoginHoyoverseUser()
    {
        if (runtimeOptions.IsWebView2Supported)
        {
            navigationService.Navigate<LoginHoyoverseUserPage>(INavigationAwaiter.Default);
        }
        else
        {
            infoBarService.Warning(SH.CoreWebView2HelperVersionUndetected);
        }
    }

    [Command("LoginByQrCodeCommand")]
    private async Task LoginByQrCode()
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();

        UserQRCodeDialog dialog = await contentDialogFactory.CreateInstanceAsync<UserQRCodeDialog>().ConfigureAwait(false);
        ValueResult<bool, UidGameToken> result = await dialog.GetUidGameTokenAsync().ConfigureAwait(false);

        if (result.TryGetValue(out UidGameToken account))
        {
            Response<LoginResult> gameTokenResp = await sessionAppClient.PostSTokenByGameTokenAsync(account).ConfigureAwait(false);

            if (gameTokenResp.IsOk())
            {
                Cookie stokenV2 = Cookie.FromLoginResult(gameTokenResp.Data);
                (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(stokenV2, false).ConfigureAwait(false);

                await HandleUserOptionResultAsync(optionResult, uid).ConfigureAwait(false);
            }
        }
    }

    [Command("RemoveUserCommand")]
    private async Task RemoveUserAsync(User? user)
    {
        if (user is not null)
        {
            try
            {
                await userService.RemoveUserAsync(user).ConfigureAwait(false);
                infoBarService.Success(SH.FormatViewModelUserRemoved(user.UserInfo?.Nickname));
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
            ArgumentNullException.ThrowIfNull(user);
            string cookieString = new StringBuilder()
                .Append(user.SToken)
                .AppendIf(user.SToken is not null, ';')
                .Append(user.LToken)
                .AppendIf(user.LToken is not null, ';')
                .Append(user.CookieToken)
                .ToString();
            serviceProvider.GetRequiredService<IClipboardProvider>().SetText(cookieString);

            ArgumentNullException.ThrowIfNull(user.UserInfo);
            infoBarService.Success(SH.FormatViewModelUserCookieCopied(user.UserInfo.Nickname));
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex);
        }
    }

    [Command("RefreshCookieTokenCommand")]
    private async Task RefreshCookieTokenAsync()
    {
        if (SelectedUser is not null)
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

    [Command("ClaimSignInRewardCommand")]
    private async Task ClaimSignInRewardAsync(AppBarButton? appBarButton)
    {
        if (SelectedUser is not null)
        {
            if (UserAndUid.TryFromUser(SelectedUser, out UserAndUid? userAndUid))
            {
                (bool isOk, string message) = await signInService.ClaimRewardAsync(userAndUid).ConfigureAwait(false);

                if (isOk)
                {
                    infoBarService.Success(message);
                }
                else
                {
                    await taskContext.SwitchToMainThreadAsync();
                    FlyoutBase.ShowAttachedFlyout(appBarButton);
                    infoBarService.Warning(message);
                }
            }
            else
            {
                infoBarService.Warning(SH.MustSelectUserAndUid);
            }
        }
    }

    [Command("OpenDocumentationCommand")]
    private async Task OpenDocumentationAsync()
    {
        await Launcher.LaunchUriAsync(new(documentationProvider.GetDocumentation()));
    }
}