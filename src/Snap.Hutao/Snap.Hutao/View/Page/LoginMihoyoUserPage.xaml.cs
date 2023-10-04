// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Bridge;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 登录米哈游通行证页面
/// </summary>
[HighQuality]
internal sealed partial class LoginMihoyoUserPage : Microsoft.UI.Xaml.Controls.Page, ISupportLoginByWebView
{
    private const string CookieHost = "https://user.mihoyo.com";
    private const string NavigateUrl = "https://user.mihoyo.com/#/login/password";

    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;

    /// <summary>
    /// 构造一个新的登录米哈游通行证页面
    /// </summary>
    public LoginMihoyoUserPage()
    {
        serviceProvider = Ioc.Default;
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        InitializeComponent();
    }

    private void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        ISupportLoginByWebView.InitialzeAsync(WebView, infoBarService, CookieHost, NavigateUrl).SafeForget();
    }

    [Command("HandleCurrentCookieCommand")]
    private async Task HandleCurrentCookieAsync()
    {
        IReadOnlyList<CoreWebView2Cookie> cookies = await WebView.CoreWebView2.CookieManager.GetCookiesAsync("https://user.mihoyo.com");
        Cookie webCookie = Cookie.FromCoreWebView2Cookies(cookies);

        if (!webCookie.TryGetCookieToken(out Cookie? loginTicketCookie))
        {
            return;
        }

        Response<ListWrapper<NameToken>> multiTokenResponse = await serviceProvider
            .GetRequiredService<AuthClient>()
            .GetMultiTokenByLoginTicketAsync(loginTicketCookie, false)
            .ConfigureAwait(false);

        if (!multiTokenResponse.IsOk())
        {
            return;
        }

        Dictionary<string, string> multiTokenMap = multiTokenResponse.Data.List.ToDictionary(n => n.Name, n => n.Token);
        Cookie stokenV1 = Cookie.FromSToken(loginTicketCookie[Cookie.LOGIN_UID], multiTokenMap[Cookie.STOKEN]);

        Response<LoginResult> loginResultResponse = await serviceProvider
            .GetRequiredService<PassportClient2>()
            .LoginBySTokenAsync(stokenV1)
            .ConfigureAwait(false);

        if (!loginResultResponse.IsOk())
        {
            return;
        }

        Cookie stokenV2 = Cookie.FromLoginResult(loginResultResponse.Data);

        await ISupportLoginByWebView
            .PostHandleCurrentCookieAsync(serviceProvider, stokenV2, false)
            .ConfigureAwait(false);
    }
}

internal interface ISupportLoginByWebView
{
    static async ValueTask InitialzeAsync(WebView2 webView2, IInfoBarService infoBarService, string cookie, string navigate)
    {
        try
        {
            await webView2.EnsureCoreWebView2Async();
            await webView2.CoreWebView2.DeleteCookiesAsync("https://user.mihoyo.com").ConfigureAwait(true);
            webView2.CoreWebView2.DisableDevToolsOnReleaseBuild();
            webView2.CoreWebView2.DisableAutoCompletion();

            webView2.CoreWebView2.Navigate("https://user.mihoyo.com/#/login/password");
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex);
        }
    }

    static async ValueTask PostHandleCurrentCookieAsync(IServiceProvider serviceProvider, Cookie cookie, bool isOversea)
    {
        (UserOptionResult result, string nickname) = await serviceProvider
            .GetRequiredService<IUserService>()
            .ProcessInputCookieAsync(cookie, false)
            .ConfigureAwait(false);

        serviceProvider.GetRequiredService<INavigationService>().GoBack();

        await serviceProvider
            .GetRequiredService<ViewModel.User.UserViewModel>()
            .HandleUserOptionResultAsync(result, nickname)
            .ConfigureAwait(false);
    }
}