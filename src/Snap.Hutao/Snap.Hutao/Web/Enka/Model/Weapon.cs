// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 武器信息
/// </summary>
[HighQuality]
internal sealed class Weapon
{
    /// <summary>
    /// 等级
    /// Weapon Level
    /// </summary>
    [JsonPropertyName("level")]
    public Level Level { get; set; }

    /// <summary>
    /// 突破等级
    /// Weapon Ascension Level
    /// </summary>
    [JsonPropertyName("promoteLevel")]
    public Level PromoteLevel { get; set; }

    /// <summary>
    /// 精炼 相较于实际等级 -1
    /// Weapon Refinement Level [0-4]
    /// </summary>
    [MaybeNull]
    [JsonPropertyName("affixMap")]
    public Dictionary<EquipAffixId, WeaponAffixLevel> AffixMap { get; set; } = default!;
}