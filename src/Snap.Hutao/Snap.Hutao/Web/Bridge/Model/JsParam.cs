// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

/// <summary>
/// 由WebView向客户端传递的参数
/// Js 参数
/// </summary>
[HighQuality]
internal sealed class JsParam
{
    /// <summary>
    /// 方法名称
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; set; } = default!;

    /// <summary>
    /// 数据 可以为空
    /// </summary>
    [JsonPropertyName("payload")]
    public JsonElement? Payload { get; set; }

    /// <summary>
    /// 回调的名称，调用 JavaScript:mhyWebBridge 时作为首个参数传入
    /// </summary>
    [JsonPropertyName("callback")]
    public string? Callback { get; set; }
}

/// <summary>
/// 由WebView向客户端传递的参数
/// Js 参数
/// </summary>
/// <typeparam name="TPayload">Payload 类型</typeparam>
[HighQuality]
[SuppressMessage("", "SA1402")]
internal sealed class JsParam<TPayload>
{
    /// <summary>
    /// 方法名称
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; set; } = default!;

    /// <summary>
    /// 数据 可以为空
    /// </summary>
    [JsonPropertyName("payload")]
    public TPayload Payload { get; set; } = default!;

    /// <summary>
    /// 回调的名称，调用 JavaScript:mhyWebBridge 时作为首个参数传入
    /// </summary>
    [JsonPropertyName("callback")]
    public string? Callback { get; set; }

    public static implicit operator JsParam<TPayload>(JsParam jsParam)
    {
        return new JsParam<TPayload>()
        {
            Method = jsParam.Method,
            Payload = jsParam.Payload.HasValue ? jsParam.Payload.Value.Deserialize<TPayload>()! : default!,
            Callback = jsParam.Callback,
        };
    }
}
