// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Geetest;

/// <summary>
/// 极验数据
/// </summary>
public class GeetestData
{
    /// <summary>
    /// 结果
    /// </summary>
    [JsonPropertyName("result")]
    public string Result { get; set; } = default!;

    /// <summary>
    /// 验证
    /// </summary>
    [JsonPropertyName("validate")]
    public string? Validate { get; set; }

    /// <summary>
    /// 分数
    /// </summary>
    public int Score { get; set; } = default!;
}