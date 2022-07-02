// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualStudio.Threading;
using Snap.Hutao.Core;
using Snap.Hutao.Extension;
using Snap.Hutao.Service.Navigation;

namespace Snap.Hutao.View.Page;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AnnouncementContentPage : Microsoft.UI.Xaml.Controls.Page
{
    // support click open browser.
    private const string MihoyoSDKDefinition =
            @"window.miHoYoGameJSSDK = {
openInBrowser: function(url){ window.chrome.webview.postMessage(url); },
openInWebview: function(url){ location.href = url }}";

    private string? targetContent;

    /// <summary>
    /// 构造一个新的公告窗体
    /// </summary>
    /// <param name="content">要展示的内容</param>
    public AnnouncementContentPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is INavigationExtra extra)
        {
            targetContent = extra.Data as string;
            LoadAnnouncementAsync(extra).SafeForget();
        }
    }

    private async Task LoadAnnouncementAsync(INavigationExtra extra)
    {
        try
        {
            await WebView.EnsureCoreWebView2Async();

            await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(MihoyoSDKDefinition);
            WebView.CoreWebView2.WebMessageReceived += (_, e) => Browser.Open(e.TryGetWebMessageAsString);
        }
        catch (Exception ex)
        {
            extra.NotifyNavigationException(ex);
            return;
        }

        WebView.NavigateToString(targetContent);
        extra.NotifyNavigationCompleted();
    }
}