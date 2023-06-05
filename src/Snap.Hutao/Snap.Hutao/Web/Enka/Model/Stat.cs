// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 圣遗物属性值
/// </summary>
internal abstract class Stat
{
    /// <summary>
    /// 属性值
    /// Property Value
    /// </summary>
    [JsonPropertyName("statValue")]
    public float StatValue { get; set; }
}