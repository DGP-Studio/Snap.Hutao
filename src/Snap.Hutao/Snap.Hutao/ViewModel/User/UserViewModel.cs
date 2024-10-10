// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.SignIn;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
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

    private AdvancedDbCollectionView<User, EntityUser>? users;

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public AdvancedDbCollectionView<User, EntityUser>? Users { get => users; set => SetProperty(ref users, value); }

    internal void HandleUserOptionResult(UserOptionResult optionResult, string uid)
    {
        switch (optionResult)
        {
            case UserOptionResult.Added:
                ArgumentNullException.ThrowIfNull(Users);
                if (Users.CurrentItem is null)
                {
                    taskContext.InvokeOnMainThread(Users.MoveCurrentToFirst);
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
                throw HutaoException.NotSupported();
        }
    }

    [Command("LoadCommand")]
    private async Task LoadAsync()
    {
        try
        {
            Users = await userService.GetUsersAsync().ConfigureAwait(true);
            Users.MoveCurrentToFirst();
        }
        catch (HutaoException ex)
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
            (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(cookie, isOversea)).ConfigureAwait(false);
            HandleUserOptionResult(optionResult, uid);
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
    private async Task LoginByQRCodeAsync()
    {
        UserQRCodeDialog dialog = await contentDialogFactory.CreateInstanceAsync<UserQRCodeDialog>().ConfigureAwait(false);
        (bool isOk, UidGameToken? token) = await dialog.GetUidGameTokenAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        Response<LoginResult> sTokenResponse = await serviceProvider
            .GetRequiredService<PassportClient2>()
            .LoginByGameTokenAsync(token)
            .ConfigureAwait(false);

        if (sTokenResponse.IsOk())
        {
            Cookie stokenV2 = Cookie.FromLoginResult(sTokenResponse.Data);
            (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(stokenV2, false)).ConfigureAwait(false);
            HandleUserOptionResult(optionResult, uid);
        }
    }

    [Command("LoginByMobileCaptchaCommand")]
    private async Task LoginByMobileCaptchaAsync()
    {
        UserMobileCaptchaDialog dialog = await contentDialogFactory.CreateInstanceAsync<UserMobileCaptchaDialog>().ConfigureAwait(false);
        if (!await dialog.GetMobileCaptchaAsync().ConfigureAwait(false))
        {
            return;
        }

        Response<LoginResult> sTokenResponse;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            PassportClient2 passportClient2 = scope.ServiceProvider.GetRequiredService<PassportClient2>();
            sTokenResponse = await passportClient2.LoginByMobileCaptchaAsync(dialog).ConfigureAwait(false);
        }

        if (sTokenResponse.IsOk())
        {
            Cookie stokenV2 = Cookie.FromLoginResult(sTokenResponse.Data);
            (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(stokenV2, false)).ConfigureAwait(false);
            HandleUserOptionResult(optionResult, uid);
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
            if (ReferenceEquals(users?.CurrentItem, user))
            {
                users.MoveCurrentToFirst();
            }

            await userService.RemoveUserAsync(user).ConfigureAwait(false);
            infoBarService.Success(SH.FormatViewModelUserRemoved(user.UserInfo?.Nickname));
        }
        catch (HutaoException ex)
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
        if (users?.CurrentItem is null)
        {
            return;
        }

        if (await userService.RefreshCookieTokenAsync(users.CurrentItem).ConfigureAwait(false))
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
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
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

        infoBarService.Warning(message);

        if (appBarButton is null)
        {
            return;
        }

        // Manual webview
        await taskContext.SwitchToMainThreadAsync();

        new ShowWebView2WindowAction()
        {
            ContentProvider = new MiHoYoJSBridgeWebView2ContentProvider()
            {
                SourceProvider = new SignInJSBridgeUriSourceProvider(),
            },
        }.ShowAt(appBarButton.XamlRoot);
    }
}