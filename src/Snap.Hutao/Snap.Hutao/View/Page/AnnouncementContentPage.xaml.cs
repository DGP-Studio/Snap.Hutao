// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core;
using Snap.Hutao.Extension;
using Snap.Hutao.Service.Navigation;
using Windows.System;

namespace Snap.Hutao.View.Page;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AnnouncementContentPage : Microsoft.UI.Xaml.Controls.Page
{
    // apply in dark mode
    private const string LightColor1 = "color:rgba(255,255,255,1)";
    private const string LightColor2 = "color:rgba(238,238,238,1)";
    private const string LightColor3 = "color:rgba(204,204,204,1)";
    private const string LightColor4 = "color:rgba(198,196,191,1)";
    private const string LightColor5 = "color:rgba(170,170,170,1)";
    private const string LightAccentColor1 = "background-color: rgb(0,40,70)";
    private const string LightAccentColor2 = "background-color: rgb(1,40,70)";

    // find in content
    private const string DarkColor1 = "color:rgba(0,0,0,1)";
    private const string DarkColor2 = "color:rgba(17,17,17,1)";
    private const string DarkColor3 = "color:rgba(51,51,51,1)";
    private const string DarkColor4 = "color:rgba(57,59,64,1)";
    private const string DarkColor5 = "color:rgba(85,85,85,1)";
    private const string DarkAccentColor1 = "background-color: rgb(255, 215, 185);";
    private const string DarkAccentColor2 = "background-color: rgb(254, 245, 231);";

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

    private static string? ReplaceForeground(string? rawContent, ElementTheme theme)
    {
        if (string.IsNullOrWhiteSpace(rawContent))
        {
            return rawContent;
        }

        if (ThemeHelper.IsDarkMode(theme, Ioc.Default.GetRequiredService<App>().RequestedTheme))
        {
            rawContent = rawContent
                .Replace(DarkColor5, LightColor5)
                .Replace(DarkColor4, LightColor4)
                .Replace(DarkColor3, LightColor3)
                .Replace(DarkColor2, LightColor2)
                .Replace(DarkAccentColor1, LightAccentColor1)
                .Replace(DarkAccentColor2, LightAccentColor2);
        }

        // wrap a default color body around
        return $@"<body style=""{(ThemeHelper.IsDarkMode(theme, Ioc.Default.GetRequiredService<App>().RequestedTheme) ? LightColor1 : DarkColor1)}"">{rawContent}</body>";
    }

    private async Task LoadAnnouncementAsync(INavigationData data)
    {
        try
        {
            await WebView.EnsureCoreWebView2Async();

            WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(MihoyoSDKDefinition);
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

    [SuppressMessage("", "VSTHRD100")]
    private async void OnWebMessageReceived(CoreWebView2 coreWebView2, CoreWebView2WebMessageReceivedEventArgs args)
    {
        string url = args.TryGetWebMessageAsString();

        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            await Launcher.LaunchUriAsync(uri).AsTask().ConfigureAwait(false);
        }
    }
}