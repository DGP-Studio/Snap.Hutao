// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Weapon;

/// <summary>
/// 武器曲线值
/// </summary>
internal sealed class WeaponTypeValue : TypeValue<FightProperty, GrowCurveType>
{
    /// <summary>
    /// 构造一个新的武器曲线值
    /// </summary>
    /// <param name="type">属性类型</param>
    /// <param name="value">曲线类型</param>
    /// <param name="initValue">初始值</param>
    public WeaponTypeValue(FightProperty type, GrowCurveType value, float initValue)
        : base(type, value)
    {
        InitValue = initValue;
    }

    /// <summary>
    /// 初始值
    /// </summary>
    public float InitValue { get; set; }
}