// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Bridge;
using Snap.Hutao.Web.WebView2;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal interface ISupportLoginByWebView
{
    [SuppressMessage("", "SH003")]
    static async Task InitialzeAsync(WebView2 webView2, IInfoBarService infoBarService, string cookie, string navigate)
    {
        try
        {
            await webView2.EnsureCoreWebView2Async();
            await webView2.CoreWebView2.DeleteCookiesAsync(cookie).ConfigureAwait(true);
            webView2.CoreWebView2.DisableDevToolsForReleaseBuild();
            webView2.CoreWebView2.DisableAutoCompletion();

            webView2.CoreWebView2.Navigate(navigate);
        }
        catch (Exception ex)
        {
            infoBarService.Error(ex);
        }
    }

    static async ValueTask PostHandleCurrentCookieAsync(IServiceProvider serviceProvider, InputCookie inputCookie)
    {
        (UserOptionResult result, string nickname) = await serviceProvider
            .GetRequiredService<IUserService>()
            .ProcessInputCookieAsync(inputCookie)
            .ConfigureAwait(false);

        serviceProvider.GetRequiredService<INavigationService>().GoBack();

        serviceProvider
            .GetRequiredService<ViewModel.User.UserViewModel>()
            .HandleUserOptionResult(result, nickname);
    }
}