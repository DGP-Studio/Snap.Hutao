// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 实时便笺验证对话框
/// </summary>
[HighQuality]
[SuppressMessage("", "CA1001")]
internal sealed partial class DailyNoteVerificationDialog : ContentDialog
{
    private readonly IServiceScope scope;
    private readonly UserAndUid userAndUid;

    private MiHoYoJSInterface? jsInterface;

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
        coreWebView2.SetCookie(user.CookieToken, user.LToken, null).SetMobileUserAgent();
        jsInterface = new(coreWebView2, scope.ServiceProvider, false);
        jsInterface.ClosePageRequested += OnClosePageRequested;

        string query = $"?role_id={userAndUid.Uid.Value}&server={userAndUid.Uid.Region}";
        coreWebView2.Navigate($"https://webstatic.mihoyo.com/app/community-game-records/index.html?bbs_presentation_style=fullscreen#/ys/daily/{query}");
    }

    private void OnClosePageRequested()
    {
        ThreadHelper.InvokeOnMainThread(Hide);
    }

    private void OnContentDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        if (jsInterface != null)
        {
            jsInterface!.ClosePageRequested -= OnClosePageRequested;
            jsInterface = null;
        }

        scope.Dispose();
    }
}