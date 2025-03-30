// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Web.Bridge;

internal sealed class SignInJSBridgeOversea : MiHoYoJSBridge
{
    // 移除 请旋转手机 提示所在的HTML元素
    /* lang=javascript */
    private const string RemoveRotationWarningScript = """
        let landscape = document.getElementById('mihoyo_landscape');
        landscape.remove();
        """;

    public SignInJSBridgeOversea(IServiceProvider serviceProvider, CoreWebView2 webView, UserAndUid userAndUid)
        : base(serviceProvider, webView, userAndUid)
    {
    }

    protected override void GetHttpRequestHeaderOverride(Dictionary<string, string> headers)
    {
        headers["x-rpc-client_type"] = "2";
    }

    protected override void DOMContentLoaded(CoreWebView2 coreWebView2)
    {
        coreWebView2.ExecuteScriptAsync(RemoveRotationWarningScript).AsTask().SafeForget();
    }
}