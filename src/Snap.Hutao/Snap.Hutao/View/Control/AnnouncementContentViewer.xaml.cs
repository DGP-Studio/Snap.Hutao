// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Control.Theme;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.System;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 公告内容页面
/// </summary>
[HighQuality]
[DependencyProperty("Announcement", typeof(Announcement))]
internal sealed partial class AnnouncementContentViewer : UserControl
{
    // apply in dark mode, Dark theme
    private const string LightColor1 = "color:rgba(255,255,255,1)";
    private const string LightColor2 = "color:rgba(238,238,238,1)";
    private const string LightColor3 = "color:rgba(204,204,204,1)";
    private const string LightColor4 = "color:rgba(198,196,191,1)";
    private const string LightColor5 = "color:rgba(170,170,170,1)";
    private const string LightAccentColor1 = "background-color: rgb(0,40,70)";
    private const string LightAccentColor2 = "background-color: rgb(1,40,70)";

    // find in content, Light theme
    private const string DarkColor1 = "color:rgba(0,0,0,1)";
    private const string DarkColor2 = "color:rgba(17,17,17,1)";
    private const string DarkColor3 = "color:rgba(51,51,51,1)";
    private const string DarkColor4 = "color:rgba(57,59,64,1)";
    private const string DarkColor5 = "color:rgba(85,85,85,1)";
    private const string DarkAccentColor1 = "background-color: rgb(255, 215, 185)";
    private const string DarkAccentColor2 = "background-color: rgb(254, 245, 231)";

    // support click open browser.
    private const string MihoyoSDKDefinition = """
        window.miHoYoGameJSSDK = {
            openInBrowser: function(url){ window.chrome.webview.postMessage(url); },
            openInWebview: function(url){ location.href = url }
        }
        """;

    private readonly RoutedEventHandler loadEventHandler;
    private readonly RoutedEventHandler unloadEventHandler;
    private readonly TypedEventHandler<CoreWebView2, CoreWebView2WebMessageReceivedEventArgs> webMessageReceivedHandler;

    /// <summary>
    /// 构造一个新的公告窗体
    /// </summary>
    public AnnouncementContentViewer()
    {
        InitializeComponent();

        loadEventHandler = OnLoaded;
        unloadEventHandler = OnUnloaded;
        webMessageReceivedHandler = OnWebMessageReceived;

        Loaded += loadEventHandler;
        Unloaded += unloadEventHandler;
    }

    private static string? GenerateHtml(Announcement? announcement, ElementTheme theme)
    {
        if (announcement is null)
        {
            return null;
        }

        string content = announcement.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        content = StyleRegex().Replace(content, string.Empty);

        bool isDarkMode = ThemeHelper.IsDarkMode(theme);

        if (isDarkMode)
        {
            StringBuilder contentBuilder = new StringBuilder(content)
                .Replace(DarkColor5, LightColor5)
                .Replace(DarkColor4, LightColor4)
                .Replace(DarkColor3, LightColor3)
                .Replace(DarkColor2, LightColor2)
                .Replace(DarkColor1, LightColor1)
                .Replace(DarkAccentColor2, LightAccentColor2)
                .Replace(DarkAccentColor1, LightAccentColor1);
            content = contentBuilder.ToString();
        }

        string document = $$"""
            <!DOCTYPE html>
            <html>

            <head>
                <style>
                    body::-webkit-scrollbar {
                        display: none;
                    }

                    img{
                        border: none;
                        vertical-align: middle;
                        width: 100%;
                    }
                </style>
            </head>

            <body style="{{(isDarkMode ? LightColor1 : DarkColor1)}}; background-color: transparent;">
                <h3>{{announcement.Title}}</h3>
                <img src="{{announcement.Banner}}" />
                <br>
                {{content}}
            </body>

            </html>
            """;

        return document;
    }

    [GeneratedRegex(" style=\"(?!\")*?vertical-align:middle;\"")]
    private static partial Regex StyleRegex();

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadAnnouncementAsync().SafeForget();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        WebView.CoreWebView2.WebMessageReceived -= webMessageReceivedHandler;
        Loaded -= loadEventHandler;
        Unloaded -= unloadEventHandler;
    }

    private async ValueTask LoadAnnouncementAsync()
    {
        try
        {
            await WebView.EnsureCoreWebView2Async();

            CoreWebView2Settings settings = WebView.CoreWebView2.Settings;
            settings.AreBrowserAcceleratorKeysEnabled = false;
            settings.AreDefaultContextMenusEnabled = false;
            settings.AreDevToolsEnabled = false;
            WebView.CoreWebView2.WebMessageReceived += webMessageReceivedHandler;

            await WebView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(MihoyoSDKDefinition);
        }
        catch (Exception)
        {
            return;
        }

        WebView.NavigateToString(GenerateHtml(Announcement, ActualTheme));
    }

    private void OnWebMessageReceived(CoreWebView2 coreWebView2, CoreWebView2WebMessageReceivedEventArgs args)
    {
        string url = args.TryGetWebMessageAsString();

        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            Launcher.LaunchUriAsync(uri).AsTask().SafeForget();
        }
    }
}