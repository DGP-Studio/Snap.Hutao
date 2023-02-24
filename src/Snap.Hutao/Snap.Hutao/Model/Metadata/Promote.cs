// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 突破加成
/// </summary>
[HighQuality]
internal sealed class Promote
{
    /// <summary>
    /// Id
    /// </summary>
    public PromoteId Id { get; set; }

    /// <summary>
    /// 突破等级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 增加的属性
    /// </summary>
    [JsonConverter(typeof(Core.Json.Converter.StringEnumKeyDictionaryConverter))]
    public Dictionary<FightProperty, float> AddProperties { get; set; } = default!;
}