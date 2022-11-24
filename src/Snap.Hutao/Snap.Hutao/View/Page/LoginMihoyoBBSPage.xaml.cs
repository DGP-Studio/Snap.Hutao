// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 登录米游社页面
/// </summary>
public sealed partial class LoginMihoyoBBSPage : Microsoft.UI.Xaml.Controls.Page
{
    private const string CookieSite = "https://bbs.mihoyo.com";
    private const string Website = "https://bbs.mihoyo.com/ys/";

    /// <summary>
    /// 构造一个新的登录米游社页面
    /// </summary>
    public LoginMihoyoBBSPage()
    {
        InitializeComponent();
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        await WebView.EnsureCoreWebView2Async();

        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie>? cookies = await manager.GetCookiesAsync(CookieSite);

        if (cookies != null)
        {
            foreach (CoreWebView2Cookie item in cookies)
            {
                manager.DeleteCookie(item);
            }
        }

        WebView.CoreWebView2.Navigate(Website);
    }

    private async Task HandleCurrentCookieAsync()
    {
        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync(CookieSite);

        Cookie cookie = Cookie.FromCoreWebView2Cookies(cookies);
        IUserService userService = Ioc.Default.GetRequiredService<IUserService>();
        (UserOptionResult result, string nickname) = await userService.ProcessInputCookieAsync(cookie).ConfigureAwait(false);

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
        HandleCurrentCookieAsync().SafeForget();
    }
}
