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
internal sealed class SignInJsInterface : MiHoYoJSInterface
{
    /// <inheritdoc cref="MiHoYoJSInterface(IServiceProvider, CoreWebView2)"/>
    public SignInJsInterface(CoreWebView2 webView, IServiceProvider serviceProvider, UserAndUid userAndUid)
        : base(webView, userAndUid)
    {
    }

    /// <inheritdoc/>
    public override JsResult<Dictionary<string, string>> GetHttpRequestHeader(JsParam param)
    {
        return new()
        {
            Data = new Dictionary<string, string>()
            {
                { "x-rpc-client_type", "2" },
                { "x-rpc-device_id",  HoyolabOptions.DeviceId },
                { "x-rpc-app_version", SaltConstants.CNVersion },
            },
        };
    }
}