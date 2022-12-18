// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Bridge;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 签到网页视图对话框
/// </summary>
public sealed partial class SignInWebViewDialog : ContentDialog
{
    private readonly IServiceScope scope;
    [SuppressMessage("", "IDE0052")]
    private SignInJsInterface? signInJsInterface;

    /// <summary>
    /// 构造一个新的签到网页视图对话框
    /// </summary>
    /// <param name="window">窗口</param>
    public SignInWebViewDialog(MainWindow window)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
        scope = Ioc.Default.CreateScope();
    }

    private void OnGridLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        InitializeAsync().SafeForget();
    }

    private async Task InitializeAsync()
    {
        await WebView.EnsureCoreWebView2Async();
        CoreWebView2 coreWebView2 = WebView.CoreWebView2;
        IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        User? user = userService.Current;

        if (user == null)
        {
            return;
        }

        coreWebView2.SetCookie(user.CookieToken, user.Ltoken, null).SetMobileUserAgent();
        signInJsInterface = new(coreWebView2, scope.ServiceProvider);
        coreWebView2.Navigate("https://webstatic.mihoyo.com/bbs/event/signin-ys/index.html?act_id=e202009291139501");
    }

    private void OnContentDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        signInJsInterface = null;
        scope.Dispose();
    }
}