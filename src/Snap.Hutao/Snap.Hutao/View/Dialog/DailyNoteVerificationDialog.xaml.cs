// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Web.Bridge;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 实时便笺认证对话框
/// </summary>
public sealed partial class DailyNoteVerificationDialog : ContentDialog
{
    private readonly IServiceScope scope;
    private readonly UserAndRole userAndRole;

    [SuppressMessage("", "IDE0052")]
    private DailyNoteJsInterface? dailyNoteJsInterface;

    /// <summary>
    /// 构造一个新的实时便笺认证对话框
    /// </summary>
    /// <param name="userAndRole">用户与角色</param>
    public DailyNoteVerificationDialog(UserAndRole userAndRole)
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
        this.userAndRole = userAndRole;
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

        Model.Entity.User user = userAndRole.User;
        coreWebView2.SetCookie(user.CookieToken, user.Ltoken, null).SetMobileUserAgent();
        dailyNoteJsInterface = new(coreWebView2, scope.ServiceProvider);

        string query = $"?role_id={userAndRole.Role.GameUid}&server={userAndRole.Role.Region}";
        coreWebView2.Navigate($"https://webstatic.mihoyo.com/app/community-game-records/index.html?bbs_presentation_style=fullscreen#/ys/daily/{query}");
    }

    private void OnContentDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        dailyNoteJsInterface = null;
        scope.Dispose();
    }
}
