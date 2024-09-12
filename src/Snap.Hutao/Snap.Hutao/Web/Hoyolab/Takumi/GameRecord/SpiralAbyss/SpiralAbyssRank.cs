﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

/// <summary>
/// 角色数值排行信息
/// </summary>
[HighQuality]
internal sealed class SpiralAbyssRank
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [JsonPropertyName("avatar_id")]
    public AvatarId AvatarId { get; set; }

    /// <summary>
    /// 角色图标
    /// </summary>
    [JsonPropertyName("avatar_icon")]
    public string AvatarIcon { get; set; } = default!;

    /// <summary>
    /// 值
    /// </summary>
    [JsonPropertyName("value")]
    public int Value { get; set; }

    /// <summary>
    /// 稀有度
    /// </summary>
    [JsonPropertyName("rarity")]
    public int Rarity { get; set; }
}