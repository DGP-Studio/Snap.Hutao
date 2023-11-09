// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge.Model;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// HoYoLAB 签到页面JS桥
/// </summary>
[HighQuality]
internal sealed class SignInJSBridgeOversea : MiHoYoJSBridge
{
    // 移除 请旋转手机 提示所在的HTML元素
    private const string RemoveRotationWarningScript = """
        let landscape = document.getElementById('mihoyo_landscape');
        landscape.remove();
        """;

    public SignInJSBridgeOversea(CoreWebView2 webView, UserAndUid userAndUid)
        : base(webView, userAndUid)
    {
    }

    protected override void GetHttpRequestHeaderCore(Dictionary<string, string> headers)
    {
        headers["x-rpc-client_type"] = "2";
    }

    protected override void DOMContentLoaded(CoreWebView2 coreWebView2)
    {
        coreWebView2.ExecuteScriptAsync(RemoveRotationWarningScript).AsTask().SafeForget();
    }
}