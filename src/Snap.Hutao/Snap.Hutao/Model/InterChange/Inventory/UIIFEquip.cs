// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Enka.Model;

namespace Snap.Hutao.Model.InterChange.Inventory;

internal class UIIFEquip
{
    /// <summary>
    /// 圣遗物
    /// Artifact Base Info
    /// </summary>
    [JsonPropertyName("reliquary")]
    public Reliquary? Reliquary { get; set; }

    /// <summary>
    /// 武器
    /// Weapon Base Info
    /// </summary>
    [JsonPropertyName("weapon")]
    public Weapon? Weapon { get; set; }
}