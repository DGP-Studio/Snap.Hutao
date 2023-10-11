// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Web.Hoyolab;
using System.Diagnostics;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// Bridge 拓展
/// </summary>
[HighQuality]
internal static class CoreWebView2Extension
{
    [Conditional("RELEASE")]
    public static void DisableDevToolsOnReleaseBuild(this CoreWebView2 webView)
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

    /// <summary>
    /// 设置 移动端UA
    /// </summary>
    /// <param name="webView">webView2</param>
    /// <returns>链式调用的WebView2</returns>
    public static CoreWebView2 SetMobileUserAgent(this CoreWebView2 webView)
    {
        webView.Settings.UserAgent = HoyolabOptions.MobileUserAgent;
        return webView;
    }

    /// <summary>
    /// 设置 移动端OsUA
    /// </summary>
    /// <param name="webView">webView2</param>
    /// <returns>链式调用的WebView2</returns>
    public static CoreWebView2 SetMobileOverseaUserAgent(this CoreWebView2 webView)
    {
        webView.Settings.UserAgent = HoyolabOptions.MobileUserAgentOversea;
        return webView;
    }

    /// <summary>
    /// 设置WebView2的Cookie
    /// </summary>
    /// <param name="webView">webView2</param>
    /// <param name="cookieToken">CookieToken</param>
    /// <param name="lToken">LToken</param>
    /// <param name="sToken">SToken</param>
    /// <param name="isOversea">是否为国际服，用于改变 cookie domain</param>
    /// <returns>链式调用的WebView2</returns>
    public static CoreWebView2 SetCookie(this CoreWebView2 webView, Cookie? cookieToken = null, Cookie? lToken = null, Cookie? sToken = null, bool isOversea = false)
    {
        CoreWebView2CookieManager cookieManager = webView.CookieManager;

        if (cookieToken is not null)
        {
            cookieManager
                .AddMihoyoCookie(Cookie.ACCOUNT_ID, cookieToken, isOversea)
                .AddMihoyoCookie(Cookie.COOKIE_TOKEN, cookieToken, isOversea);
        }

        if (lToken is not null)
        {
            cookieManager
                .AddMihoyoCookie(Cookie.LTUID, lToken, isOversea)
                .AddMihoyoCookie(Cookie.LTOKEN, lToken, isOversea);
        }

        if (sToken is not null)
        {
            cookieManager
                .AddMihoyoCookie(Cookie.MID, sToken, isOversea)
                .AddMihoyoCookie(Cookie.STUID, sToken, isOversea)
                .AddMihoyoCookie(Cookie.STOKEN, sToken, isOversea);
        }

        return webView;
    }

    private static CoreWebView2CookieManager AddMihoyoCookie(this CoreWebView2CookieManager manager, string name, Cookie cookie, bool isOversea = false)
    {
        string domain = isOversea ? ".hoyolab.com" : ".mihoyo.com";
        manager.AddOrUpdateCookie(manager.CreateCookie(name, cookie[name], domain, "/"));
        return manager;
    }
}