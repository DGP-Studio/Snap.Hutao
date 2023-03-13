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
[HighQuality]
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
            IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://www.hoyolab.com");
            foreach (CoreWebView2Cookie item in cookies)
            {
                manager.DeleteCookie(item);
            }

            WebView.CoreWebView2.Navigate("https://www.hoyolab.com");
        }
        catch (Exception ex)
        {
            Ioc.Default.GetRequiredService<IInfoBarService>().Error(ex);
        }
    }

    private async Task HandleCurrentCookieAsync(CancellationToken token)
    {
        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://www.hoyolab.com");

        Cookie hoyoLabCookie = Cookie.FromCoreWebView2Cookies(cookies);

        // 处理 cookie 并添加用户
        (UserOptionResult result, string nickname) = await Ioc.Default
            .GetRequiredService<IUserService>()
            .ProcessInputOsCookieAsync(hoyoLabCookie)
            .ConfigureAwait(false);

        Ioc.Default.GetRequiredService<INavigationService>().GoBack();
        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();

        switch (result)
        {
            case UserOptionResult.Added:
                ViewModel.UserViewModel vm = Ioc.Default.GetRequiredService<ViewModel.UserViewModel>();
                if (vm.Users!.Count == 1)
                {
                    await ThreadHelper.SwitchToMainThreadAsync();
                    vm.SelectedUser = vm.Users.Single();
                }

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
