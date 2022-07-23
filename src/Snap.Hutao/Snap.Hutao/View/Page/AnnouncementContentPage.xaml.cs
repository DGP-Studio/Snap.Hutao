// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
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
    private const string LightColor1 = "color:rgba(255,255,255,1)";
    private const string LightColor2 = "color:rgba(238,238,238,1)";
    private const string LightColor3 = "color:rgba(204,204,204,1)";
    private const string LightColor4 = "color:rgba(198,196,191,1)";
    private const string LightColor5 = "color:rgba(170,170,170,1)";

    private const string DarkColor1 = "color:rgba(0,0,0,1)";
    private const string DarkColor2 = "color:rgba(17,17,17,1)";
    private const string DarkColor3 = "color:rgba(51,51,51,1)";
    private const string DarkColor4 = "color:rgba(57,59,64,1)";
    private const string DarkColor5 = "color:rgba(85,85,85,1)";

    // support click open browser.
    private const string MihoyoSDKDefinition =
        @"window.miHoYoGameJSSDK = {
openInBrowser: function(url){ window.chrome.webview.postMessage(url); },
openInWebview: function(url){ location.href = url }}";

    private string? targetContent;

    /// <summary>
    /// 构造一个新的公告窗体
    /// </summary>
    public AnnouncementContentPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is INavigationData extra)
        {
            targetContent = extra.Data as string;
            LoadAnnouncementAsync(extra).SafeForget();
        }
    }

    private async Task LoadAnnouncementAsync(INavigationData data)
    {
        try
        {
            await WebView.EnsureCoreWebView2Async();

            await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(MihoyoSDKDefinition);
            WebView.CoreWebView2.WebMessageReceived += (_, e) => Browser.Open(e.TryGetWebMessageAsString());
        }
        catch (Exception ex)
        {
            data.NotifyNavigationException(ex);
            return;
        }

        WebView.NavigateToString(ReplaceForeground(targetContent, ActualTheme));
        data.NotifyNavigationCompleted();
    }

    private void PageActualThemeChanged(FrameworkElement sender, object args)
    {
        WebView.NavigateToString(ReplaceForeground(targetContent, ActualTheme));
    }

    private string? ReplaceForeground(string? rawContent, ElementTheme theme)
    {
        if (string.IsNullOrWhiteSpace(rawContent))
        {
            return rawContent;
        }

        bool isDarkMode = theme switch
        {
            ElementTheme.Default => App.Current.RequestedTheme == ApplicationTheme.Dark,
            ElementTheme.Dark => true,
            _ => false,
        };

        if (isDarkMode)
        {
            rawContent = rawContent
                .Replace(DarkColor5, LightColor5)
                .Replace(DarkColor4, LightColor4)
                .Replace(DarkColor3, LightColor3)
                .Replace(DarkColor2, LightColor2);
        }

        // wrap a default color body around
        return $@"<body style=""{(isDarkMode ? LightColor1 : DarkColor1)}"">{rawContent}</body>";
    }
}