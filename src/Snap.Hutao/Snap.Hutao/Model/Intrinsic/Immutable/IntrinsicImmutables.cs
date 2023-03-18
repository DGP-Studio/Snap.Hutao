// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Intrinsic.Immutable;

/// <summary>
/// 本地化的不可变的原生枚举
/// </summary>
[HighQuality]
internal static class IntrinsicImmutables
{
    private static readonly ImmutableHashSet<string> associationTypes = Enum.GetValues<AssociationType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToImmutableHashSet();
    private static readonly ImmutableHashSet<string> weaponTypes = Enum.GetValues<WeaponType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToImmutableHashSet();
    private static readonly ImmutableHashSet<string> itemQualities = Enum.GetValues<ItemQuality>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToImmutableHashSet();
    private static readonly ImmutableHashSet<string> bodyTypes = Enum.GetValues<BodyType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToImmutableHashSet();
    private static readonly ImmutableHashSet<string> fightProperties = Enum.GetValues<FightProperty>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToImmutableHashSet();
    private static readonly ImmutableHashSet<string> elementNames = new HashSet<string>(7)
    {
        SH.ModelIntrinsicElementNameFire,
        SH.ModelIntrinsicElementNameWater,
        SH.ModelIntrinsicElementNameGrass,
        SH.ModelIntrinsicElementNameElec,
        SH.ModelIntrinsicElementNameWind,
        SH.ModelIntrinsicElementNameIce,
        SH.ModelIntrinsicElementNameRock,
    }.ToImmutableHashSet();

    /// <summary>
    /// 所属地区
    /// </summary>
    public static ImmutableHashSet<string> AssociationTypes { get => associationTypes; }

    /// <summary>
    /// 武器类型
    /// </summary>
    public static ImmutableHashSet<string> WeaponTypes { get => weaponTypes; }

    /// <summary>
    /// 物品类型
    /// </summary>
    public static ImmutableHashSet<string> ItemQualities { get => itemQualities; }

    /// <summary>
    /// 身材类型
    /// </summary>
    public static ImmutableHashSet<string> BodyTypes { get => bodyTypes; }

    /// <summary>
    /// 战斗属性
    /// </summary>
    public static ImmutableHashSet<string> FightProperties { get => fightProperties; }

    /// <summary>
    /// 元素名称
    /// </summary>
    public static ImmutableHashSet<string> ElementNames { get => elementNames; }
}