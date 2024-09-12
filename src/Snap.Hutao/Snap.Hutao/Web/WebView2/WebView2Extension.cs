﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using System.Diagnostics;

namespace Snap.Hutao.Web.WebView2;

internal static class WebView2Extension
{
    [Conditional("RELEASE")]
    public static void DisableDevToolsForReleaseBuild(this CoreWebView2 webView)
    {
        CoreWebView2Settings settings = webView.Settings;
        settings.AreBrowserAcceleratorKeysEnabled = false;
        settings.AreDefaultContextMenusEnabled = false;
        settings.AreDevToolsEnabled = false;
    }

    public static void DisableAutoCompletion(this CoreWebView2 webView)
    {
        CoreWebView2Settings settings = webView.Settings;
        settings.IsGeneralAutofillEnabled = false;
        settings.IsPasswordAutosaveEnabled = false;
    }

    public static async ValueTask DeleteCookiesAsync(this CoreWebView2 webView, string url)
    {
        CoreWebView2CookieManager manager = webView.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync(url);
        foreach (CoreWebView2Cookie item in cookies)
        {
            manager.DeleteCookie(item);
        }
    }

    public static bool IsDisposed(this Microsoft.UI.Xaml.Controls.WebView2 webView2)
    {
        return WinRTExtension.IsDisposed(webView2);
    }
}