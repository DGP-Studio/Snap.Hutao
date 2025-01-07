// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.SignIn;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.Data.Converter.Specialized;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Text;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.ViewModel.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class UserViewModel : ObservableObject
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly CultureOptions cultureOptions;
    private readonly ISignInService signInService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;

    private AdvancedDbCollectionView<User, EntityUser>? users;

    public partial RuntimeOptions RuntimeOptions { get; }

    public AdvancedDbCollectionView<User, EntityUser>? Users { get => users; set => SetProperty(ref users, value); }

    public ImmutableArray<NameValue<OverseaThirdPartyKind>> OverseaThirdPartyKinds { get; } = ImmutableCollectionsNameValue.FromEnum<OverseaThirdPartyKind>(static kind => kind is OverseaThirdPartyKind.Twitter ? ThirdPartyIconConverter.TwitterName : kind.ToString());

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
                ArgumentNullException.ThrowIfNull(Users);
                taskContext.InvokeOnMainThread(Users.Refresh);
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

    [Command("LoginByPasswordOverseaCommand")]
    private async Task LoginByPasswordOverseaAsync()
    {
        await taskContext.SwitchToMainThreadAsync();

        UserAccountPasswordDialog dialog = await contentDialogFactory
            .CreateInstanceAsync<UserAccountPasswordDialog>()
            .ConfigureAwait(false);
        ValueResult<bool, LoginResult?> result = await dialog.LoginAsync(true).ConfigureAwait(false);

        if (result.TryGetValue(out LoginResult? loginResult))
        {
            Cookie stokenV2 = Cookie.FromLoginResult(loginResult);
            (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(stokenV2, true)).ConfigureAwait(false);
            HandleUserOptionResult(optionResult, uid);
        }
    }

    [Command("LoginByThirdPartyOverseaCommand")]
    private async Task LoginByThirdPartyOverseaAsync(OverseaThirdPartyKind kind)
    {
        await taskContext.SwitchToMainThreadAsync();
        OverseaThirdPartyLoginWebView2ContentProvider contentProvider = new(kind, cultureOptions.LanguageCode);
        ShowWebView2WindowAction.Show(contentProvider, currentXamlWindowReference.GetXamlRoot());

        await taskContext.SwitchToBackgroundAsync();
        ThirdPartyToken? token = await contentProvider.GetResultAsync().ConfigureAwait(false);

        if (token is null)
        {
            return;
        }

        Response<LoginResult> response;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHoyoPlayPassportClient hoyoPlayPassportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IHoyoPlayPassportClient>>().Create(true);
            response = await hoyoPlayPassportClient.LoginByThirdPartyAsync(token).ConfigureAwait(false);
        }

        if (ResponseValidator.TryValidate(response, infoBarService, out LoginResult? loginResult))
        {
            Cookie stokenV2 = Cookie.FromLoginResult(loginResult);
            (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(stokenV2, true)).ConfigureAwait(false);
            HandleUserOptionResult(optionResult, uid);
        }
    }

    private async ValueTask AddUserByManualInputCookieAsync(bool isOversea)
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();

        // Get cookie from user input
        UserDialog dialog = await contentDialogFactory.CreateInstanceAsync<UserDialog>().ConfigureAwait(false);
        ValueResult<bool, string> result = await dialog.GetInputCookieAsync().ConfigureAwait(false);

        // User confirms the input
        if (result.TryGetValue(out string? rawCookie))
        {
            Cookie cookie = Cookie.Parse(rawCookie);
            (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(cookie, isOversea)).ConfigureAwait(false);
            HandleUserOptionResult(optionResult, uid);
        }
    }

    [Command("LoginByQRCodeCommand")]
    private async Task LoginByQRCodeAsync()
    {
        UserQRCodeDialog dialog = await contentDialogFactory.CreateInstanceAsync<UserQRCodeDialog>().ConfigureAwait(false);
        (bool isOk, QrLoginResult? qrLoginResult) = await dialog.GetQrLoginResultAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        Cookie stokenV2 = Cookie.FromQrLoginResult(qrLoginResult);
        (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(stokenV2, false)).ConfigureAwait(false);
        HandleUserOptionResult(optionResult, uid);
    }

    [Command("LoginByMobileCaptchaCommand")]
    private async Task LoginByMobileCaptchaAsync()
    {
        UserMobileCaptchaDialog dialog = await contentDialogFactory.CreateInstanceAsync<UserMobileCaptchaDialog>().ConfigureAwait(false);
        if (!await dialog.GetMobileCaptchaAsync().ConfigureAwait(false))
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IPassportClient passportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IPassportClient>>().Create(false);
            Response<LoginResult> response = await passportClient.LoginByMobileCaptchaAsync(dialog).ConfigureAwait(false);

            if (ResponseValidator.TryValidate(response, scope.ServiceProvider, out LoginResult? loginResult))
            {
                Cookie stokenV2 = Cookie.FromLoginResult(loginResult);
                (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(stokenV2, false)).ConfigureAwait(false);
                HandleUserOptionResult(optionResult, uid);
            }
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
    private async Task CopyCookieAsync(User? user)
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
            await serviceProvider.GetRequiredService<IClipboardProvider>().SetTextAsync(cookieString).ConfigureAwait(false);

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
    private async Task ClaimSignInRewardAsync()
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

        // Manual webview
        await taskContext.SwitchToMainThreadAsync();

        MiHoYoJSBridgeWebView2ContentProvider provider = new()
        {
            SourceProvider = new SignInJSBridgeUriSourceProvider(),
        };

        ShowWebView2WindowAction.Show(provider, currentXamlWindowReference.GetXamlRoot());
    }
}