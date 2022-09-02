// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

/// <summary>
/// 圣遗物套装效果
/// </summary>
public class ReliquaryAffix
{
    /// <summary>
    /// 激活个数
    /// </summary>
    [JsonPropertyName("activation_number")]
    public int ActivationNumber { get; set; }

    /// <summary>
    /// 效果描述
    /// </summary>
    [JsonPropertyName("effect")]
    public string? Effect { get; set; }
}