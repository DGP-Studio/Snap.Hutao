// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Converter;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Reliquary;

/// <summary>
/// 圣遗物等级
/// </summary>
public class ReliquaryLevel
{
    /// <summary>
    /// 品质
    /// </summary>
    public ItemQuality Quality { get; set; }

    /// <summary>
    /// 等级 1-21
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 属性
    /// </summary>
    [JsonConverter(typeof(StringEnumKeyDictionaryConverter))]
    public Dictionary<FightProperty, double> Properties { get; set; } = default!;
}