// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// 实时便笺页面调用桥
/// </summary>
public class DailyNoteJsInterface : MiHoYoJSInterface
{
    /// <summary>
    /// 构造一个新的实时便笺页面调用桥
    /// </summary>
    /// <param name="webView">webview</param>
    /// <param name="serviceProvider">服务提供器</param>
    public DailyNoteJsInterface(CoreWebView2 webView, IServiceProvider serviceProvider)
        : base(webView, serviceProvider)
    {
    }
}