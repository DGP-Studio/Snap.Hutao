// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Reliquary;

/// <summary>
/// 属性映射
/// </summary>
internal sealed partial class ReliquaryMainAffixLevel
{
    private Dictionary<FightProperty, float>? propertyMap;

    /// <summary>
    /// 属性映射
    /// </summary>
    public Dictionary<FightProperty, float> PropertyMap
    {
        get => propertyMap ??= Properties.ToDictionary(t => t.Type, t => t.Value);
    }
}
