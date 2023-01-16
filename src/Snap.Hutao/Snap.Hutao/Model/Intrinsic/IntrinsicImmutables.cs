// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Intrinsic;

/// <summary>
/// 不可变的原生枚举
/// </summary>
public static class IntrinsicImmutables
{
    /// <summary>
    /// 所属地区
    /// </summary>
    public static readonly ImmutableList<string> AssociationTypes = Enum.GetValues<AssociationType>().Select(e => e.GetDescriptionOrNull()).OfType<string>().ToImmutableList();

    /// <summary>
    /// 武器类型
    /// </summary>
    public static readonly ImmutableList<string> WeaponTypes = Enum.GetValues<WeaponType>().Select(e => e.GetDescriptionOrNull()).OfType<string>().ToImmutableList();

    /// <summary>
    /// 物品类型
    /// </summary>
    public static readonly ImmutableList<string> ItemQualities = Enum.GetValues<ItemQuality>().Select(e => e.GetDescriptionOrNull()).OfType<string>().ToImmutableList();

    /// <summary>
    /// 身材类型
    /// </summary>
    public static readonly ImmutableList<string> BodyTypes = Enum.GetValues<BodyType>().Select(e => e.GetDescriptionOrNull()).OfType<string>().ToImmutableList();

    /// <summary>
    /// 战斗属性
    /// </summary>
    public static readonly ImmutableList<string> FightProperties = Enum.GetValues<FightProperty>().Select(e => e.GetDescriptionOrNull()).OfType<string>().ToImmutableList();
}