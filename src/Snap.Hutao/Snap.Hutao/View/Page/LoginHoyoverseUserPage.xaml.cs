// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 登录米哈游通行证页面
/// </summary>
internal sealed partial class LoginHoyoverseUserPage : Microsoft.UI.Xaml.Controls.Page
{
    /// <summary>
    /// 构造一个新的登录米哈游通行证页面
    /// </summary>
    public LoginHoyoverseUserPage()
    {
        InitializeComponent();
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await WebView.EnsureCoreWebView2Async();

            CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
            IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://account.hoyolab.com");
            foreach (CoreWebView2Cookie item in cookies)
            {
                manager.DeleteCookie(item);
            }

            WebView.CoreWebView2.Navigate("https://account.hoyolab.com/#/login");
        }
        catch (Exception ex)
        {
            Ioc.Default.GetRequiredService<IInfoBarService>().Error(ex);
        }
    }

    private async Task HandleCurrentCookieAsync(CancellationToken token = default)
    {
        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://account.hoyolab.com");

        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();

        // Get user id from text input, login_uid is missed in cookie
        string uid = UidInputText.Text;

        if (uid.Length != 9)
        {
            await ThreadHelper.SwitchToMainThreadAsync();
            infoBarService.Information(SH.ViewPageLoginHoyoverseUserHint);
            return;
        }

        Cookie loginTicketCookie = Cookie.FromCoreWebView2Cookies(cookies);
        loginTicketCookie["login_uid"] = uid;

        // 使用 loginTicket 获取 stoken
        Response<ListWrapper<NameToken>> multiTokenResponse = await Ioc.Default
            .GetRequiredService<AuthClient>()
            .GetMultiTokenByLoginTicketAsync(loginTicketCookie, true, token)
            .ConfigureAwait(false);

        if (!multiTokenResponse.IsOk())
        {
            return;
        }

        Dictionary<string, string> multiTokenMap = multiTokenResponse.Data.List.ToDictionary(n => n.Name, n => n.Token);
        Cookie hoyoLabCookie = Cookie.Parse($"{Cookie.STUID}={uid};{Cookie.STOKEN}={multiTokenMap[Cookie.STOKEN]}");

        // 处理 cookie 并添加用户
        (UserOptionResult result, string nickname) = await Ioc.Default
            .GetRequiredService<IUserService>()
            .ProcessInputCookieAsync(hoyoLabCookie, true)
            .ConfigureAwait(false);

        Ioc.Default.GetRequiredService<INavigationService>().GoBack();

        switch (result)
        {
            case UserOptionResult.Added:
                ViewModel.UserViewModel vm = Ioc.Default.GetRequiredService<ViewModel.UserViewModel>();
                if (vm.Users!.Count == 1)
                {
                    await ThreadHelper.SwitchToMainThreadAsync();
                    vm.SelectedUser = vm.Users.Single();
                }

                infoBarService.Success(string.Format(SH.ViewModelUserAdded, nickname));
                break;
            case UserOptionResult.Incomplete:
                infoBarService.Information(SH.ViewModelUserIncomplete);
                break;
            case UserOptionResult.Invalid:
                infoBarService.Information(SH.ViewModelUserInvalid);
                break;
            case UserOptionResult.Updated:
                infoBarService.Success(string.Format(SH.ViewModelUserUpdated, nickname));
                break;
            default:
                throw Must.NeverHappen();
        }
    }

    private void CookieButtonClick(object sender, RoutedEventArgs e)
    {
        HandleCurrentCookieAsync().SafeForget();
    }
}
