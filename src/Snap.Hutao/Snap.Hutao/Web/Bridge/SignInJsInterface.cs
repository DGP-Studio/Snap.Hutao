// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Web.Bridge.Model;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// 签到页面JS桥
/// </summary>
[HighQuality]
internal sealed class SignInJsInterface : MiHoYoJSInterface
{
    /// <inheritdoc cref="MiHoYoJSInterface(CoreWebView2, IServiceProvider)"/>
    public SignInJsInterface(CoreWebView2 webView, IServiceProvider serviceProvider)
        : base(webView, serviceProvider)
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
                { "x-rpc-device_id",  Core.HoyolabOptions.DeviceId },
                { "x-rpc-app_version", Core.HoyolabOptions.XrpcVersion },
            },
        };
    }
}