// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Control.Theme;
using Snap.Hutao.Web.Bridge;
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
    // support click open browser.
    private const string MihoyoSDKDefinition = """
        window.miHoYoGameJSSDK = {
            openInBrowser: function(url){ window.chrome.webview.postMessage(url); },
            openInWebview: function(url){ location.href = url }
        }
        """;

    private static readonly Dictionary<string, string> DarkLightReverts = new()
    {
        { "color:rgba(0,0,0,1)", "color:rgba(255,255,255,1)" },
        { "color:rgba(17,17,17,1)", "color:rgba(238,238,238,1)" },
        { "color:rgba(51,51,51,1)", "color:rgba(204,204,204,1)" },
        { "color:rgba(57,59,64,1)", "color:rgba(198,196,191,1)" },
        { "color:rgba(85,85,85,1)", "color:rgba(170,170,170,1)" },
        { "background-color: rgb(255, 215, 185)", "background-color: rgb(0,40,70)" },
        { "background-color: rgb(254, 245, 231)", "background-color: rgb(1,40,70)" },
        { "background-color:rgb(244, 244, 245)", "background-color:rgba(11, 11, 10)" },
    };

    private readonly RoutedEventHandler loadEventHandler;
    private readonly RoutedEventHandler unloadEventHandler;
    private readonly TypedEventHandler<CoreWebView2, CoreWebView2WebMessageReceivedEventArgs> webMessageReceivedHandler;

    /// <summary>
    /// 构造一个新的公告窗体
    /// </summary>
    public AnnouncementContentViewer()
    {
        Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00000000");
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
            StringBuilder contentBuilder = new(content);

            foreach ((string dark, string light) in DarkLightReverts)
            {
                contentBuilder.Replace(dark, light);
            }

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

                    img {
                        border: none;
                        vertical-align: middle;
                        width: 100%;
                    }
                </style>
            </head>

            <body style="{{(isDarkMode ? "color:rgba(255,255,255,1)" : "color:rgba(0,0,0,1)")}}; background-color: transparent;">
                <h3>{{announcement.Title}}</h3>
                <img src="{{announcement.Banner}}"/>
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
        if (WebView is { CoreWebView2: CoreWebView2 coreWebView2 })
        {
            coreWebView2.WebMessageReceived -= webMessageReceivedHandler;
        }
    }

    private async ValueTask LoadAnnouncementAsync()
    {
        try
        {
            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.DisableDevToolsForReleaseBuild();
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