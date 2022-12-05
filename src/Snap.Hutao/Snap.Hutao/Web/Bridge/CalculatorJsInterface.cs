// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// 养成计算器调用桥
/// </summary>
public class CalculatorJsInterface : MiHoYoJSInterface
{
    /// <inheritdoc cref="MiHoYoJSInterface(CoreWebView2, IServiceProvider)"/>
    public CalculatorJsInterface(CoreWebView2 webView, IServiceProvider serviceProvider)
        : base(webView, serviceProvider)
    {
    }
}