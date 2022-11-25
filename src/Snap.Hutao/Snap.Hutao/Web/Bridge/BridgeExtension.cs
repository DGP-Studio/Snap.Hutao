// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Web.Hoyolab;
using WinRT;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// Bridge 拓展
/// </summary>
public static class BridgeExtension
{
    private const string InitializeJsInterfaceScript = """
        let c = {};
        c.postMessage = str => chrome.webview.hostObjects.MiHoYoJsBridge.OnMessage(str);
        c.closePage = () => c.postMessage('{"method":"closePage"}');
        window.MiHoYoJSInterface = c;
        """;

    private const string HideScrollBarScript = """
        let st = document.createElement('style');
        st.innerHTML = '::-webkit-scrollbar{display:none}';
        document.querySelector('body').appendChild(st);
        """;

    /// <summary>
    /// 设置 移动端UA
    /// </summary>
    /// <param name="webView">webview2</param>
    public static void SetMobileUserAgent(this CoreWebView2 webView)
    {
        webView.Settings.UserAgent = "Mozilla/5.0 (Linux; Android 12) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/106.0.5249.126 Mobile Safari/537.36 miHoYoBBS/2.41.0";
    }

    /// <summary>
    /// 初始化调用桥
    /// </summary>
    /// <param name="webView">webview2</param>
    /// <param name="logger">日志器</param>
    /// <param name="checkHost">检查主机</param>
    /// <returns>初始化后的调用桥</returns>
    public static MiHoYoJsBridge InitializeBridge(this CoreWebView2 webView, ILogger<MiHoYoJsBridge> logger, bool checkHost = true)
    {
        MiHoYoJsBridge bridge = new(webView, logger);
        var result = webView.As<ICoreWebView2Interop>().AddHostObjectToScript("MiHoYoJsBridge", bridge);

        webView.DOMContentLoaded += OnDOMContentLoaded;
        webView.NavigationStarting += (coreWebView2, args) => OnWebViewNavigationStarting(coreWebView2, args, checkHost);

        return bridge;
    }

    /// <summary>
    /// 设置WebView2的Cookie
    /// </summary>
    /// <param name="webView">webview2</param>
    /// <param name="cookieToken">CookieToken</param>
    /// <param name="ltoken">Ltoken</param>
    /// <param name="stoken">Stoken</param>
    /// <returns>链式调用的WebView2</returns>
    public static CoreWebView2 SetCookie(this CoreWebView2 webView, Cookie? cookieToken = null, Cookie? ltoken = null, Cookie? stoken = null)
    {
        CoreWebView2CookieManager cookieManager = webView.CookieManager;

        if (cookieToken != null)
        {
            cookieManager.AddMihoyoCookie("account_id", cookieToken).AddMihoyoCookie("cookie_token", cookieToken);
        }

        if (ltoken != null)
        {
            cookieManager.AddMihoyoCookie("ltuid", ltoken).AddMihoyoCookie("ltoken", ltoken);
        }

        if (stoken != null)
        {
            cookieManager.AddMihoyoCookie("stuid", stoken).AddMihoyoCookie("stoken", stoken);
        }

        return webView;
    }

    private static CoreWebView2CookieManager AddMihoyoCookie(this CoreWebView2CookieManager manager, string name, Cookie cookie)
    {
        manager.AddOrUpdateCookie(manager.CreateCookie(name, cookie[name], ".mihoyo.com", "/"));
        return manager;
    }

    [SuppressMessage("", "VSTHRD100")]
    private static async void OnDOMContentLoaded(CoreWebView2 coreWebView2, CoreWebView2DOMContentLoadedEventArgs args)
    {
        string result = await coreWebView2.ExecuteScriptAsync(HideScrollBarScript);
        _ = result;
    }

    [SuppressMessage("", "VSTHRD100")]
    private static async void OnWebViewNavigationStarting(CoreWebView2 coreWebView2, CoreWebView2NavigationStartingEventArgs args, bool checkHost)
    {
        if (!checkHost || new Uri(args.Uri).Host.EndsWith("mihoyo.com"))
        {
            string result = await coreWebView2.ExecuteScriptAsync(InitializeJsInterfaceScript);
            _ = result;
        }
    }
}