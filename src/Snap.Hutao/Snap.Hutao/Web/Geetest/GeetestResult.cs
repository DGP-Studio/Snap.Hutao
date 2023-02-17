// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Geetest;

/// <summary>
/// 极验返回结果信息
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
internal sealed class GeetestResult<T>
{
    /// <summary>
    /// 成功失败的标识码 success
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = default!;

    /// <summary>
    /// 返回数据，json格式
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;
}
