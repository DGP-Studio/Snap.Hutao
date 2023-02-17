// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 认证基本信息
/// </summary>
[HighQuality]
internal class Certification
{
    /// <summary>
    /// 认证类型
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; set; } = default!;
}
