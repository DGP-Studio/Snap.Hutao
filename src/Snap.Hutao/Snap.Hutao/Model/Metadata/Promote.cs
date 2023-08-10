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
    private Dictionary<FightProperty, float>? addPropertyMap;

    /// <summary>
    /// Id
    /// </summary>
    public PromoteId Id { get; set; }

    /// <summary>
    /// 突破等级
    /// </summary>
    public PromoteLevel Level { get; set; }

    /// <summary>
    /// 增加的属性
    /// </summary>
    public List<TypeValue<FightProperty, float>> AddProperties { get; set; } = default!;

    public float GetValue(FightProperty property)
    {
        addPropertyMap ??= AddProperties.ToDictionary(a => a.Type, a => a.Value);
        return addPropertyMap.GetValueOrDefault(property);
    }
}