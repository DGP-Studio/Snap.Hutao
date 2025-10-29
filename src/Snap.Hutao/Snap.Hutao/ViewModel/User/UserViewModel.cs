// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Notification;
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

[GeneratedConstructor]
[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class UserViewModel : ObservableObject
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly CultureOptions cultureOptions;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IMessenger messenger;

    public partial RuntimeOptions RuntimeOptions { get; }

    [ObservableProperty]
    public partial AdvancedDbCollectionView<User, EntityUser>? Users { get; set; }

    public ImmutableArray<NameValue<OverseaThirdPartyKind>> OverseaThirdPartyKinds { get; } = ImmutableCollectionsNameValue.FromEnum<OverseaThirdPartyKind>(static kind => kind is OverseaThirdPartyKind.Twitter ? ThirdPartyIconConverter.TwitterName : kind.ToString());

    public IProperty<bool> IsViewUnloaded { get; } = Property.Create(false);

    internal void HandleUserOptionResult(UserOptionResultKind optionResultKind, string? uid)
    {
        switch (optionResultKind)
        {
            case UserOptionResultKind.Added:
                ArgumentNullException.ThrowIfNull(Users);
                if (Users.CurrentItem is null)
                {
                    taskContext.InvokeOnMainThread(Users.MoveCurrentToFirst);
                }

                messenger.Send(InfoBarMessage.Success(SH.FormatViewModelUserAdded(uid)));
                break;
            case UserOptionResultKind.CookieIncomplete:
                messenger.Send(InfoBarMessage.Information(SH.ViewModelUserIncomplete));
                break;
            case UserOptionResultKind.CookieInvalid:
                messenger.Send(InfoBarMessage.Information(SH.ViewModelUserInvalid));
                break;
            case UserOptionResultKind.CookieUpdated:
                ArgumentNullException.ThrowIfNull(Users);
                taskContext.InvokeOnMainThread(Users.Refresh);
                messenger.Send(InfoBarMessage.Success(SH.FormatViewModelUserUpdated(uid)));
                break;
            case UserOptionResultKind.GameRoleNotFound:
                messenger.Send(InfoBarMessage.Information(SH.ViewModelUserEmptyGameRole));
                break;
            default:
                throw HutaoException.NotSupported();
        }
    }

    [Command("LoadCommand")]
    private async Task LoadAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Load", "UserViewModel.Command"));

        try
        {
            Users = await userService.GetUsersAsync().ConfigureAwait(true);
        }
        catch (HutaoException ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    [Command("AddUserCommand")]
    private Task AddUserAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Add chinese user", "UserViewModel.Command", [("source", "Manual Input")]));
        return AddUserByManualInputCookieAsync(false).AsTask();
    }

    [Command("AddOverseaUserCommand")]
    private Task AddOverseaUserAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Add oversea user", "UserViewModel.Command", [("source", "Manual Input")]));
        return AddUserByManualInputCookieAsync(true).AsTask();
    }

    [Command("LoginByPasswordOverseaCommand")]
    private async Task LoginByPasswordOverseaAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Add oversea user", "UserViewModel.Command", [("source", "Password")]));

        await taskContext.SwitchToMainThreadAsync();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            UserAccountPasswordDialog dialog = await contentDialogFactory
                .CreateInstanceAsync<UserAccountPasswordDialog>(scope.ServiceProvider)
                .ConfigureAwait(false);
            ValueResult<bool, LoginResult?> result = await dialog.LoginAsync(true).ConfigureAwait(false);

            if (result.TryGetValue(out LoginResult? loginResult))
            {
                Cookie stokenV2 = Cookie.FromLoginResult(loginResult);
                (UserOptionResultKind optionResult, string? uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(stokenV2, true)).ConfigureAwait(false);
                HandleUserOptionResult(optionResult, uid);
            }
        }
    }

    [Command("LoginByThirdPartyOverseaCommand")]
    private async Task LoginByThirdPartyOverseaAsync(OverseaThirdPartyKind kind)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Add oversea user", "UserViewModel.Command", [("source", "Third Party"), ("kind", kind.ToString())]));

        await taskContext.SwitchToMainThreadAsync();
        if (currentXamlWindowReference.GetXamlRoot() is not { } xamlRoot)
        {
            return;
        }

        OverseaThirdPartyLoginWebView2ContentProvider contentProvider = new(kind, cultureOptions.LanguageCode);
        ShowWebView2WindowAction.Show(contentProvider, xamlRoot);

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
            IUserVerificationService userVerificationService = scope.ServiceProvider.GetRequiredService<IUserVerificationService>();

            (string? rawRisk, response) = await hoyoPlayPassportClient.LoginByThirdPartyAsync(token).ConfigureAwait(false);

            if (await userVerificationService.TryVerifyAsync(token, rawRisk, true).ConfigureAwait(false))
            {
                (_, response) = await hoyoPlayPassportClient.LoginByThirdPartyAsync(token).ConfigureAwait(false);
            }
        }

        if (ResponseValidator.TryValidate(response, messenger, out LoginResult? loginResult))
        {
            Cookie sTokenV2 = Cookie.FromLoginResult(loginResult);
            (UserOptionResultKind optionResult, string? uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(sTokenV2, true)).ConfigureAwait(false);
            HandleUserOptionResult(optionResult, uid);
        }
    }

    private async ValueTask AddUserByManualInputCookieAsync(bool isOversea)
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            // Get cookie from user input
            UserDialog dialog = await contentDialogFactory.CreateInstanceAsync<UserDialog>(scope.ServiceProvider).ConfigureAwait(false);
            ValueResult<bool, string?> result = await dialog.GetInputCookieAsync().ConfigureAwait(false);

            // User confirms the input
            if (result.TryGetValue(out string? rawCookie))
            {
                Cookie cookie = Cookie.Parse(rawCookie);
                (UserOptionResultKind optionResult, string? uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(cookie, isOversea)).ConfigureAwait(false);
                HandleUserOptionResult(optionResult, uid);
            }
        }
    }

    [Command("LoginByQRCodeCommand")]
    private async Task LoginByQRCodeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Add chinese user", "UserViewModel.Command", [("source", "QR Code")]));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            UserQRCodeDialog dialog = await contentDialogFactory.CreateInstanceAsync<UserQRCodeDialog>(scope.ServiceProvider).ConfigureAwait(false);
            (bool isOk, QrLoginResult? qrLoginResult) = await dialog.GetQrLoginResultAsync().ConfigureAwait(false);

            if (!isOk)
            {
                return;
            }

            Cookie sTokenV2 = Cookie.FromQrLoginResult(qrLoginResult);
            (UserOptionResultKind optionResult, string? uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(sTokenV2, false)).ConfigureAwait(false);
            HandleUserOptionResult(optionResult, uid);
        }
    }

    [Command("LoginByMobileCaptchaCommand")]
    private async Task LoginByMobileCaptchaAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Add chinese user", "UserViewModel.Command", [("source", "Mobile Captcha")]));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            UserMobileCaptchaDialog dialog = await contentDialogFactory.CreateInstanceAsync<UserMobileCaptchaDialog>(scope.ServiceProvider).ConfigureAwait(false);
            if (!await dialog.GetMobileCaptchaAsync().ConfigureAwait(false))
            {
                return;
            }

            IPassportClient passportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IPassportClient>>().Create(false);
            Response<LoginResult> response = await passportClient.LoginByMobileCaptchaAsync(dialog).ConfigureAwait(false);

            if (ResponseValidator.TryValidate(response, scope.ServiceProvider, out LoginResult? loginResult))
            {
                Cookie sTokenV2 = Cookie.FromLoginResult(loginResult);
                (UserOptionResultKind optionResult, string? uid) = await userService.ProcessInputCookieAsync(InputCookie.CreateForDeviceFpInference(sTokenV2, false)).ConfigureAwait(false);
                HandleUserOptionResult(optionResult, uid);
            }
        }
    }

    [Command("RemoveUserCommand")]
    private async Task RemoveUserAsync(User? user)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove user", "UserViewModel.Command"));

        if (user is null)
        {
            return;
        }

        try
        {
            if (ReferenceEquals(Users?.CurrentItem, user))
            {
                Users.MoveCurrentToFirst();
            }

            await userService.RemoveUserAsync(user).ConfigureAwait(false);
            messenger.Send(InfoBarMessage.Success(SH.FormatViewModelUserRemoved(user.UserInfo?.Nickname)));
        }
        catch (HutaoException ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    [Command("CopyCookieCommand")]
    private async Task CopyCookieAsync(User? user)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Copy cookie", "UserViewModel.Command"));

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
            messenger.Send(InfoBarMessage.Success(SH.FormatViewModelUserCookieCopied(user.UserInfo.Nickname)));
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    [Command("RefreshCookieTokenCommand")]
    private async Task RefreshCookieTokenAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh cookie token", "UserViewModel.Command"));

        if (Users?.CurrentItem is null)
        {
            return;
        }

        _ = await userService.RefreshCookieTokenAsync(Users.CurrentItem).ConfigureAwait(false)
            ? messenger.Send(InfoBarMessage.Success(SH.ViewUserRefreshCookieTokenSuccess))
            : messenger.Send(InfoBarMessage.Warning(SH.ViewUserRefreshCookieTokenWarning));
    }
}