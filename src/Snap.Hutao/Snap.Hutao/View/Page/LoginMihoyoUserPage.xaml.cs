// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 登录米哈游通行证页面
/// </summary>
public sealed partial class LoginMihoyoUserPage : Microsoft.UI.Xaml.Controls.Page
{
    /// <summary>
    /// 构造一个新的登录米哈游通行证页面
    /// </summary>
    public LoginMihoyoUserPage()
    {
        InitializeComponent();
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        await WebView.EnsureCoreWebView2Async();

        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://user.mihoyo.com");
        foreach (CoreWebView2Cookie item in cookies)
        {
            manager.DeleteCookie(item);
        }

        WebView.CoreWebView2.Navigate("https://user.mihoyo.com/#/login/password");
    }

    private async Task HandleCurrentCookieAsync(CancellationToken token)
    {
        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://user.mihoyo.com");

        Cookie loginTicketCookie = Cookie.FromCoreWebView2Cookies(cookies);
        Response<ListWrapper<NameToken>> multiTokenResponse = await Ioc.Default
            .GetRequiredService<AuthClient>()
            .GetMultiTokenByLoginTicketAsync(loginTicketCookie, token)
            .ConfigureAwait(false);

        if (!multiTokenResponse.IsOk())
        {
            return;
        }

        Dictionary<string, string> multiTokenMap = multiTokenResponse.Data.List.ToDictionary(n => n.Name, n => n.Token);

        Cookie stokenV1 = Cookie.Parse($"stuid={loginTicketCookie["login_uid"]};stoken={multiTokenMap["stoken"]}");
        Response<LoginResult> loginResultResponse = await Ioc.Default
            .GetRequiredService<PassportClient2>()
            .LoginByStokenAsync(stokenV1, token)
            .ConfigureAwait(false);

        if (!loginResultResponse.IsOk())
        {
            return;
        }

        Cookie stokenV2 = Cookie.FromLoginResult(loginResultResponse.Data);
        (UserOptionResult result, string nickname) = await Ioc.Default
            .GetRequiredService<IUserService>()
            .ProcessInputCookieAsync(stokenV2)
            .ConfigureAwait(false);

        Ioc.Default.GetRequiredService<INavigationService>().GoBack();
        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();

        switch (result)
        {
            case UserOptionResult.Added:
                infoBarService.Success($"用户 [{nickname}] 添加成功");
                break;
            case UserOptionResult.Incomplete:
                infoBarService.Information($"此 Cookie 不完整，操作失败");
                break;
            case UserOptionResult.Invalid:
                infoBarService.Information($"此 Cookie 无效，操作失败");
                break;
            case UserOptionResult.Updated:
                infoBarService.Success($"用户 [{nickname}] 更新成功");
                break;
            default:
                throw Must.NeverHappen();
        }
    }

    private void CookieButtonClick(object sender, RoutedEventArgs e)
    {
        HandleCurrentCookieAsync(CancellationToken.None).SafeForget();
    }
}
