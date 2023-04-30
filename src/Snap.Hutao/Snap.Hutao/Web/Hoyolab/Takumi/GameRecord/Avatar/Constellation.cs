// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

/// <summary>
/// 命座
/// </summary>
[HighQuality]
internal sealed class Constellation
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonPropertyName("id")]
    public SkillId Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 效果描述
    /// </summary>
    [JsonPropertyName("effect")]
    public string Effect { get; set; } = default!;

    /// <summary>
    /// 是否激活
    /// </summary>
    [JsonPropertyName("is_actived")]
    public bool IsActived { get; set; }

    /// <summary>
    /// 位置
    /// </summary>
    [JsonPropertyName("pos")]
    public int Position { get; set; }
}