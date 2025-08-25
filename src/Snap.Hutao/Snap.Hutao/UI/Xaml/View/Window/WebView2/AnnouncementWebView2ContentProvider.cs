// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.Collections.Frozen;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Graphics;
using Windows.System;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

[DependencyProperty<Announcement>("Announcement")]
internal sealed partial class AnnouncementWebView2ContentProvider : DependencyObject, IWebView2ContentProvider
{
    // support click open browser.
    private const string MihoyoSDKDefinition = """
        window.miHoYoGameJSSDK = {
            openInBrowser: function(url){ window.chrome.webview.postMessage(url); },
            openInWebview: function(url){ location.href = url }
        }
        """;

    private static readonly FrozenDictionary<string, string> DarkLightReverts = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create("color:rgba(0,0,0,1)", "color:rgba(255,255,255,1)"),
        KeyValuePair.Create("color:rgba(17,17,17,1)", "color:rgba(238,238,238,1)"),
        KeyValuePair.Create("color:rgba(51,51,51,1)", "color:rgba(204,204,204,1)"),
        KeyValuePair.Create("color:rgba(57,59,64,1)", "color:rgba(198,196,191,1)"),
        KeyValuePair.Create("color:rgba(73,73,73,1)", "color:rgba(182,182,182,1)"),
        KeyValuePair.Create("color:rgba(85,85,85,1)", "color:rgba(170,170,170,1)"),
        KeyValuePair.Create("background-color: rgb(255, 215, 185)", "background-color: rgb(0,40,70)"),
        KeyValuePair.Create("background-color: rgb(254, 245, 231)", "background-color: rgb(1,40,70)"),
        KeyValuePair.Create("background-color:rgb(244, 244, 245)", "background-color:rgba(11, 11, 10)"),
    ]);

    public ElementTheme ActualTheme { get; set; }

    public CoreWebView2? CoreWebView2 { get; set; }

    public Action? CloseWindowAction { get; set; }

    [GeneratedRegex(" style=\"(?!\")*?vertical-align:middle;\"")]
    private static partial Regex StyleRegex { get; }

    [GeneratedRegex("[0-9]+\\.[0-9]+rem")]
    private static partial Regex RemRegex { get; }

    public async ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(CoreWebView2);

        try
        {
            CoreWebView2.WebMessageReceived += OnWebMessageReceived;
            await CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(MihoyoSDKDefinition);
        }
        catch
        {
            CoreWebView2.WebMessageReceived -= OnWebMessageReceived;
            return;
        }

        LoadAnnouncement(CoreWebView2);
    }

    public void Unload()
    {
        if (CoreWebView2 is not null)
        {
            CoreWebView2.WebMessageReceived -= OnWebMessageReceived;
        }
    }

    public RectInt32 InitializePosition(RectInt32 parentRect, double parentDpi)
    {
        // Shrink 48 px on each side
        return WebView2WindowPosition.Padding(parentRect, 48);
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

        content = StyleRegex.Replace(content, string.Empty);
        content = RemRegex.Replace(content, "calc($0 * 10)");

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
                <title>{{announcement.Subtitle}} - {{announcement.Title}}</title>
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

    private void LoadAnnouncement(CoreWebView2 coreWebView2)
    {
        try
        {
            coreWebView2.NavigateToString(GenerateHtml(Announcement, ActualTheme));
        }
        catch (COMException ex)
        {
            // 组或资源的状态不是执行请求操作的正确状态。
            if (!HutaoNative.IsWin32(ex.ErrorCode, WIN32_ERROR.ERROR_INVALID_STATE))
            {
                throw;
            }
        }
    }

    private void OnWebMessageReceived(CoreWebView2 coreWebView2, CoreWebView2WebMessageReceivedEventArgs args)
    {
        string url = args.TryGetWebMessageAsString();

        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri? uri))
        {
            _ = Launcher.LaunchUriAsync(uri);
        }
    }
}