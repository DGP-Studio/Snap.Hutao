// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

/// <summary>
/// 用于传回网页的参数
/// Js结果
/// </summary>
/// <typeparam name="TData">内部数据类型</typeparam>
[HighQuality]
internal sealed class JsResult<TData> : IJsResult
{
    /// <summary>
    /// 代码
    /// </summary>
    [JsonPropertyName("retcode")]
    public int Code { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 数据
    /// </summary>
    [JsonPropertyName("data")]
    public TData Data { get; set; } = default!;

    /// <inheritdoc/>
    string IJsResult.ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}