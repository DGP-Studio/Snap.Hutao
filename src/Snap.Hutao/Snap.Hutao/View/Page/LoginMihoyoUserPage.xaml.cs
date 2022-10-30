// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Hoyolab;

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
        WebView.CoreWebView2.SourceChanged += OnCoreWebView2SourceChanged;

        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://user.mihoyo.com");
        foreach (CoreWebView2Cookie item in cookies)
        {
            manager.DeleteCookie(item);
        }

        WebView.CoreWebView2.Navigate("https://user.mihoyo.com/#/login/password");
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void OnCoreWebView2SourceChanged(CoreWebView2 sender, CoreWebView2SourceChangedEventArgs args)
    {
        if (sender != null)
        {
            if (sender.Source.ToString() == "https://user.mihoyo.com/#/account/home")
            {
                await HandleCurrentCookieAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task HandleCurrentCookieAsync()
    {
        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://user.mihoyo.com");
        WebView.CoreWebView2.SourceChanged -= OnCoreWebView2SourceChanged;

        Cookie cookie = Cookie.FromCoreWebView2Cookies(cookies);
        IUserService userService = Ioc.Default.GetRequiredService<IUserService>();
        (UserOptionResult result, string nickname) = await userService.ProcessInputCookieAsync(cookie).ConfigureAwait(false);

        Ioc.Default.GetRequiredService<INavigationService>().GoBack();
        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();

        if (result == UserOptionResult.Upgraded)
        {
            infoBarService.Information($"用户 [{nickname}] 的 Cookie 升级成功");
        }
        else
        {
            infoBarService.Warning("请先添加对应用户的米游社Cookie");
        }
    }

    private void CookieButtonClick(object sender, RoutedEventArgs e)
    {
        HandleCurrentCookieAsync().SafeForget();
    }
}
