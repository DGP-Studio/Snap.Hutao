// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 签到网页视图对话框
/// </summary>
[HighQuality]
internal sealed partial class SignInWebViewDialog : ContentDialog
{
    private readonly IServiceScope scope;
    [SuppressMessage("", "IDE0052")]
    private MiHoYoJSInterface? jsInterface;

    /// <summary>
    /// 构造一个新的签到网页视图对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public SignInWebViewDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        XamlRoot = serviceProvider.GetRequiredService<MainWindow>().Content.XamlRoot;

        scope = serviceProvider.CreateScope();
    }

    private void OnGridLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        InitializeAsync().SafeForget();
    }

    private async Task InitializeAsync()
    {
        await WebView.EnsureCoreWebView2Async();
        CoreWebView2 coreWebView2 = WebView.CoreWebView2;
        User? user = scope.ServiceProvider.GetRequiredService<IUserService>().Current;

        if (user == null)
        {
            return;
        }

        if (user.Entity.IsOversea)
        {
            coreWebView2.SetCookie(user.CookieToken, user.LToken, null, true).SetMobileOverseaUserAgent();
            jsInterface = new SignInJSInterfaceOversea(coreWebView2, scope.ServiceProvider);
            coreWebView2.Navigate("https://act.hoyolab.com/ys/event/signin-sea-v3/index.html?act_id=e202102251931481");
        }
        else
        {
            coreWebView2.SetCookie(user.CookieToken, user.LToken, null, false).SetMobileUserAgent();
            jsInterface = new SignInJsInterface(coreWebView2, scope.ServiceProvider);
            coreWebView2.Navigate("https://webstatic.mihoyo.com/bbs/event/signin-ys/index.html?act_id=e202009291139501");
        }
    }

    private void OnContentDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        jsInterface = null;
        scope.Dispose();
    }
}