// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
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
using Snap.Hutao.Web.Response;
using System.Text;
using EntityUser = Snap.Hutao.Model.Entity.User;

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
    private readonly INavigationService navigationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ISignInService signInService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;

    private User? selectedUser;
    private ObservableReorderableDbCollection<User, EntityUser>? users;

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    /// <summary>
    /// 当前选择的用户信息
    /// </summary>
    public User? SelectedUser
    {
        get => selectedUser ??= userService.Current;
        set
        {
            if (value is { SelectedUserGameRole: null })
            {
                // Pre select the chosen role to avoid multiple UserChangedMessage
                value.SetSelectedUserGameRole(value.UserGameRoles.FirstOrFirstOrDefault(role => role.IsChosen), false);
            }

            if (SetProperty(ref selectedUser, value))
            {
                userService.Current = value;
            }
        }
    }

    /// <summary>
    /// 用户信息集合
    /// </summary>
    public ObservableReorderableDbCollection<User, EntityUser>? Users { get => users; set => SetProperty(ref users, value); }

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
            case UserOptionResult.CookieIncomplete:
                infoBarService.Information(SH.ViewModelUserIncomplete);
                break;
            case UserOptionResult.CookieInvalid:
                infoBarService.Information(SH.ViewModelUserInvalid);
                break;
            case UserOptionResult.CookieUpdated:
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
        return AddUserByManualInputCookieAsync(false).AsTask();
    }

    [Command("AddOverseaUserCommand")]
    private Task AddOverseaUserAsync()
    {
        return AddUserByManualInputCookieAsync(true).AsTask();
    }

    private async ValueTask AddUserByManualInputCookieAsync(bool isOversea)
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
            (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateWithDeviceFpInference(cookie, isOversea)).ConfigureAwait(false);
            await HandleUserOptionResultAsync(optionResult, uid).ConfigureAwait(false);
        }
    }

    [Command("LoginMihoyoUserCommand")]
    private void LoginMihoyoUser()
    {
        NavigateToLoginPage<LoginMihoyoUserPage>();
    }

    [Command("LoginHoyoverseUserCommand")]
    private void LoginHoyoverseUser()
    {
        NavigateToLoginPage<LoginHoyoverseUserPage>();
    }

    private void NavigateToLoginPage<TPage>()
        where TPage : Page
    {
        if (runtimeOptions.IsWebView2Supported)
        {
            navigationService.Navigate<TPage>(INavigationAwaiter.Default);
        }
        else
        {
            infoBarService.Warning(SH.CoreWebView2HelperVersionUndetected);
        }
    }

    [Command("LoginByQRCodeCommand")]
    private async Task LoginByQRCode()
    {
        UserQRCodeDialog dialog = await contentDialogFactory.CreateInstanceAsync<UserQRCodeDialog>().ConfigureAwait(false);
        (bool isOk, UidGameToken? token) = await dialog.GetUidGameTokenAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        Response<LoginResult> sTokenResponse = await serviceProvider
            .GetRequiredService<PassportClient2>()
            .GetSTokenByGameTokenAsync(token)
            .ConfigureAwait(false);

        if (sTokenResponse.IsOk())
        {
            Cookie stokenV2 = Cookie.FromLoginResult(sTokenResponse.Data);
            (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateWithDeviceFpInference(stokenV2, false)).ConfigureAwait(false);
            await HandleUserOptionResultAsync(optionResult, uid).ConfigureAwait(false);
        }
    }

    [Command("RemoveUserCommand")]
    private async Task RemoveUserAsync(User? user)
    {
        if (user is null)
        {
            return;
        }

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
        if (SelectedUser is null)
        {
            return;
        }

        if (await userService.RefreshCookieTokenAsync(SelectedUser).ConfigureAwait(false))
        {
            infoBarService.Success(SH.ViewUserRefreshCookieTokenSuccess);
        }
        else
        {
            infoBarService.Warning(SH.ViewUserRefreshCookieTokenWarning);
        }
    }

    [Command("ClaimSignInRewardCommand")]
    private async Task ClaimSignInRewardAsync(AppBarButton? appBarButton)
    {
        if (!UserAndUid.TryFromUser(SelectedUser, out UserAndUid? userAndUid))
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        (bool isOk, string message) = await signInService.ClaimRewardAsync(userAndUid).ConfigureAwait(false);

        if (isOk)
        {
            infoBarService.Success(message);
            return;
        }

        // Manual webview
        await taskContext.SwitchToMainThreadAsync();
        FlyoutBase.ShowAttachedFlyout(appBarButton);
        infoBarService.Warning(message);
    }
}