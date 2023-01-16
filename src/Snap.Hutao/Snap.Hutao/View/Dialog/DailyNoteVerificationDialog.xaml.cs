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
/// 实时便笺验证对话框
/// </summary>
[SuppressMessage("", "CA1001")]
public sealed partial class DailyNoteVerificationDialog : ContentDialog
{
    private readonly IServiceScope scope;
    private readonly UserAndUid userAndUid;

    private DailyNoteJsInterface? dailyNoteJsInterface;

    /// <summary>
    /// 构造一个新的实时便笺验证对话框
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    public DailyNoteVerificationDialog(UserAndUid userAndUid)
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
        this.userAndUid = userAndUid;
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

        Model.Entity.User user = userAndUid.User;
        coreWebView2.SetCookie(user.CookieToken, user.Ltoken, null).SetMobileUserAgent();
        dailyNoteJsInterface = new(coreWebView2, scope.ServiceProvider);
        dailyNoteJsInterface.ClosePageRequested += OnClosePageRequested;

        string query = $"?role_id={userAndUid.Uid.Value}&server={userAndUid.Uid.Region}";
        coreWebView2.Navigate($"https://webstatic.mihoyo.com/app/community-game-records/index.html?bbs_presentation_style=fullscreen#/ys/daily/{query}");
    }

    private void OnClosePageRequested()
    {
        ThreadHelper.InvokeOnMainThread(Hide);
    }

    private void OnContentDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        if (dailyNoteJsInterface != null)
        {
            dailyNoteJsInterface!.ClosePageRequested -= OnClosePageRequested;
            dailyNoteJsInterface = null;
        }

        scope.Dispose();
    }
}