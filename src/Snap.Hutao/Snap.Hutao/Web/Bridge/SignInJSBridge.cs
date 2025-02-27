// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Web.Bridge;

internal sealed class SignInJSBridge : MiHoYoJSBridge
{
    public SignInJSBridge(IServiceProvider serviceProvider, CoreWebView2 webView, UserAndUid userAndUid)
        : base(serviceProvider, webView, userAndUid)
    {
    }

    protected override void GetHttpRequestHeaderOverride(Dictionary<string, string> headers)
    {
        headers["x-rpc-client_type"] = "2";
    }
}