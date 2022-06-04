// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

/// <summary>
/// 角色详细详细
/// </summary>
public class Character : Avatar
{
    /// <summary>
    /// 角色图片
    /// </summary>
    [Obsolete("we don't want to use this ugly pic here")]
    [JsonPropertyName("image")]
    public new string Image { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 武器
    /// </summary>
    [JsonPropertyName("weapon")]
    public Weapon Weapon { get; set; } = default!;

    /// <summary>
    /// 圣遗物
    /// </summary>
    [JsonPropertyName("reliquaries")]
    public List<Reliquary> Reliquaries { get; set; } = default!;

    /// <summary>
    /// 命座
    /// </summary>
    [JsonPropertyName("constellations")]
    public List<Constellation> Constellations { get; set; } = default!;

    /// <summary>
    /// 时装
    /// </summary>
    [JsonPropertyName("costumes")]
    public List<Costume> Costumes { get; set; } = default!;
}
