// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Bridge;
using Snap.Hutao.Web.Bridge.Model;
using Snap.Hutao.Web.Bridge.Model.Event;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Windows.UI.Popups;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 签到网页视图对话框
/// </summary>
public sealed partial class SignInWebViewDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的签到网页视图对话框
    /// </summary>
    /// <param name="window">窗口</param>
    public SignInWebViewDialog(MainWindow window)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
    }

    private void OnGridLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        InitializeAsync().SafeForget();
    }

    private async Task InitializeAsync()
    {
        await WebView.EnsureCoreWebView2Async();
        CoreWebView2 coreWebView2 = WebView.CoreWebView2;
        IUserService userService = Ioc.Default.GetRequiredService<IUserService>();
        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();
        ILogger<MiHoYoJsBridge> logger = Ioc.Default.GetRequiredService<ILogger<MiHoYoJsBridge>>();
        User? user = userService.Current;

        coreWebView2.SetCookie(user?.CookieToken, user?.Ltoken);
        coreWebView2.SetMobileUserAgent();
        coreWebView2.InitializeBridge(logger, false)
            .Register<JsEventClosePage>(e => Hide())
            .Register<JsEventRealPersonValidation>(e => infoBarService.Information("无法使用此功能", "请前往米游社进行实名认证后重试"))
            .Register<JsEventGetStatusBarHeight>(s => s.Callback(result => result.Data["statusBarHeight"] = 0))
            .Register<JsEventGetDynamicSecretV1>(s => s.Callback(result =>
            {
                result.Data["DS"] = DynamicSecretHandler.GetDynamicSecret(nameof(SaltType.K2), nameof(DynamicSecretVersion.Gen1), includeChars: true);
            }))
            .Register<JsEventGetUserInfo>(s => s.Callback(result =>
            {
                result.Data["id"] = "111";
                result.Data["gender"] = 0;
                result.Data["nickname"] = "222";
                result.Data["introduce"] = "333";
                result.Data["avatar_url"] = "https://img-static.mihoyo.com/communityweb/upload/52de23f1b1a060e4ccaa8b24c1305dd9.png";
            }));

        coreWebView2.OpenDevToolsWindow();
        coreWebView2.Navigate("https://webstatic.mihoyo.com/bbs/event/signin-ys/index.html?act_id=e202009291139501");
    }
}
