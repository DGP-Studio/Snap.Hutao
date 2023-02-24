// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Binding.BaseValue;

/// <summary>
/// 战斗属性与初始值
/// </summary>
internal sealed class PropertyCurveValue
{
    /// <summary>
    /// 构造一个新的战斗属性与初始值
    /// </summary>
    /// <param name="property">战斗属性</param>
    /// <param name="type">类型</param>
    /// <param name="value">初始值</param>
    public PropertyCurveValue(FightProperty property, GrowCurveType type, float value)
    {
        Property = property;
        Type = type;
        Value = value;
    }

    /// <summary>
    /// 战斗属性值
    /// </summary>
    public FightProperty Property { get; }

    /// <summary>
    /// 曲线类型
    /// </summary>
    public GrowCurveType Type { get; }

    /// <summary>
    /// 初始值
    /// </summary>
    public float Value { get; }
}
