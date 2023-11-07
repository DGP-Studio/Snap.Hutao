// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge.Model;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// 签到页面JS桥
/// </summary>
[HighQuality]
internal sealed class SignInJSInterface : MiHoYoJSInterface
{
    /// <inheritdoc cref="MiHoYoJSInterface(IServiceProvider, CoreWebView2)"/>
    public SignInJSInterface(CoreWebView2 webView, IServiceProvider serviceProvider, UserAndUid userAndUid)
        : base(webView, userAndUid)
    {
    }

    protected override void GetHttpRequestHeaderCore(Dictionary<string, string> headers)
    {
        headers["x-rpc-client_type"] = "2";
    }
}