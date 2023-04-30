// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge;
using Snap.Hutao.Web.Request.QueryString;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 实时便笺验证对话框
/// </summary>
[HighQuality]
[SuppressMessage("", "CA1001")]
internal sealed partial class DailyNoteVerificationDialog : ContentDialog
{
    private readonly ITaskContext taskContext;
    private readonly IServiceScope scope;
    private readonly UserAndUid userAndUid;

    private MiHoYoJSInterface? jsInterface;

    /// <summary>
    /// 构造一个新的实时便笺验证对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="userAndUid">用户与角色</param>
    public DailyNoteVerificationDialog(IServiceProvider serviceProvider, UserAndUid userAndUid)
    {
        InitializeComponent();
        XamlRoot = serviceProvider.GetRequiredService<MainWindow>().Content.XamlRoot;

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        scope = serviceProvider.CreateScope();
        this.userAndUid = userAndUid;
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
        jsInterface = new(coreWebView2, scope.ServiceProvider);
        jsInterface.ClosePageRequested += OnClosePageRequested;

        QueryString query = userAndUid.Uid.ToQueryString();
        coreWebView2.Navigate($"https://webstatic.mihoyo.com/app/community-game-records/index.html?bbs_presentation_style=fullscreen#/ys/daily/?{query}");
    }

    private void OnClosePageRequested()
    {
        taskContext.InvokeOnMainThread(Hide);
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