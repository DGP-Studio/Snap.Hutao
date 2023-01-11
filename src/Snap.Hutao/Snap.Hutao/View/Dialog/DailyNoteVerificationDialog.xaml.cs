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
/// ʵʱ�����֤�Ի���
/// </summary>
public sealed partial class DailyNoteVerificationDialog : ContentDialog
{
    private readonly IServiceScope scope;
    private readonly UserAndUid userAndUid;

    [SuppressMessage("", "IDE0052")]
    private DailyNoteJsInterface? dailyNoteJsInterface;

    /// <summary>
    /// ����һ���µ�ʵʱ�����֤�Ի���
    /// </summary>
    /// <param name="userAndUid">�û����ɫ</param>
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

        string query = $"?role_id={userAndUid.Uid.Value}&server={userAndUid.Uid.Region}";
        coreWebView2.Navigate($"https://webstatic.mihoyo.com/app/community-game-records/index.html?bbs_presentation_style=fullscreen#/ys/daily/{query}");
    }

    private void OnContentDialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        dailyNoteJsInterface = null;
        scope.Dispose();
    }
}
