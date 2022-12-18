// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Bridge;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 实时便笺认证对话框
/// </summary>
public sealed partial class DailyNoteVerificationDialog : ContentDialog
{
    private readonly IServiceScope scope;
    private readonly User user;
    private readonly PlayerUid uid;
    [SuppressMessage("", "IDE0052")]
    private DailyNoteJsInterface? dailyNoteJsInterface;

    /// <summary>
    /// 构造一个新的实时便笺认证对话框
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="user">用户</param>
    /// <param name="uid">uid</param>
    public DailyNoteVerificationDialog(Window window, User user, PlayerUid uid)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
        this.user = user;
        this.uid = uid;
        scope = Ioc.Default.CreateScope();
    }

    private void OnGridLoaded(object sender, RoutedEventArgs e)
    {
        InitializeAsync().SafeForget();
    }

    private async Task InitializeAsync()
    {
        await WebView.EnsureCoreWebView2Async();
        CoreWebView2 coreWebView2 = WebView.CoreWebView2;

        coreWebView2.SetCookie(user.CookieToken, user.Ltoken, null).SetMobileUserAgent();
        dailyNoteJsInterface = new(coreWebView2, scope.ServiceProvider);

        string query = $"?role_id={uid.Value}&server={uid.Region}";
        coreWebView2.Navigate($"https://webstatic.mihoyo.com/app/community-game-records/index.html?bbs_presentation_style=fullscreen#/ys/daily/{query}");
    }

    private void OnContentDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        dailyNoteJsInterface = null;
        scope.Dispose();
    }
}
