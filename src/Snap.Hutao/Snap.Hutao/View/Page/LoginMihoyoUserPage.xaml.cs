// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 登录米哈游通行证页面
/// </summary>
[HighQuality]
internal sealed partial class LoginMihoyoUserPage : Microsoft.UI.Xaml.Controls.Page
{
    /// <summary>
    /// 构造一个新的登录米哈游通行证页面
    /// </summary>
    public LoginMihoyoUserPage()
    {
        InitializeComponent();
    }

    private async void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await WebView.EnsureCoreWebView2Async();

            CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
            IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://user.mihoyo.com");
            foreach (CoreWebView2Cookie item in cookies)
            {
                manager.DeleteCookie(item);
            }

            CoreWebView2 coreWebView2 = WebView.CoreWebView2;
            coreWebView2.Settings.IsGeneralAutofillEnabled = false;
            coreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            coreWebView2.Navigate("https://user.mihoyo.com/#/login/password");
        }
        catch (Exception ex)
        {
            Ioc.Default.GetRequiredService<IInfoBarService>().Error(ex);
        }
    }

    [Command("HandleCurrentCookieCommand")]
    private async Task HandleCurrentCookieAsync()
    {
        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://user.mihoyo.com");

        Cookie loginTicketCookie = Cookie.FromCoreWebView2Cookies(cookies);
        Response<ListWrapper<NameToken>> multiTokenResponse = await Ioc.Default
            .GetRequiredService<AuthClient>()
            .GetMultiTokenByLoginTicketAsync(loginTicketCookie, false)
            .ConfigureAwait(false);

        if (!multiTokenResponse.IsOk())
        {
            return;
        }

        Dictionary<string, string> multiTokenMap = multiTokenResponse.Data.List.ToDictionary(n => n.Name, n => n.Token);

        Cookie stokenV1 = Cookie.FromSToken(loginTicketCookie[Cookie.LOGIN_UID], multiTokenMap[Cookie.STOKEN]);

        Response<LoginResult> loginResultResponse = await Ioc.Default
            .GetRequiredService<PassportClient2>()
            .LoginBySTokenAsync(stokenV1)
            .ConfigureAwait(false);

        if (!loginResultResponse.IsOk())
        {
            return;
        }

        Cookie stokenV2 = Cookie.FromLoginResult(loginResultResponse.Data);
        (UserOptionResult result, string nickname) = await Ioc.Default
            .GetRequiredService<IUserService>()
            .ProcessInputCookieAsync(stokenV2, false)
            .ConfigureAwait(false);

        Ioc.Default.GetRequiredService<INavigationService>().GoBack();

        await Ioc.Default
            .GetRequiredService<ViewModel.User.UserViewModel>()
            .HandleUserOptionResultAsync(result, nickname)
            .ConfigureAwait(false);
    }
}
