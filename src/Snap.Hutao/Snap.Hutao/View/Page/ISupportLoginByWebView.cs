// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Bridge;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.View.Page;

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