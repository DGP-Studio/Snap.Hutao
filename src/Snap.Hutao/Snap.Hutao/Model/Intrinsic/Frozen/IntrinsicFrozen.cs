// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Frozen;

namespace Snap.Hutao.Model.Intrinsic.Frozen;

/// <summary>
/// 本地化的不可变的原生枚举
/// </summary>
[HighQuality]
internal static class IntrinsicFrozen
{
    /// <summary>
    /// 所属地区
    /// </summary>
    public static FrozenSet<string> AssociationTypes { get; } = Enum.GetValues<AssociationType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToFrozenSet();

    /// <summary>
    /// 武器类型
    /// </summary>
    public static FrozenSet<string> WeaponTypes { get; } = Enum.GetValues<WeaponType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToFrozenSet();

    /// <summary>
    /// 物品类型
    /// </summary>
    public static FrozenSet<string> ItemQualities { get; } = Enum.GetValues<QualityType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToFrozenSet();

    /// <summary>
    /// 身材类型
    /// </summary>
    public static FrozenSet<string> BodyTypes { get; } = Enum.GetValues<BodyType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToFrozenSet();

    /// <summary>
    /// 战斗属性
    /// </summary>
    public static FrozenSet<string> FightProperties { get; } = Enum.GetValues<FightProperty>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToFrozenSet();

    /// <summary>
    /// 元素名称
    /// </summary>
    public static FrozenSet<string> ElementNames { get; } = FrozenSet.ToFrozenSet(
    [
        SH.ModelIntrinsicElementNameFire,
        SH.ModelIntrinsicElementNameWater,
        SH.ModelIntrinsicElementNameGrass,
        SH.ModelIntrinsicElementNameElec,
        SH.ModelIntrinsicElementNameWind,
        SH.ModelIntrinsicElementNameIce,
        SH.ModelIntrinsicElementNameRock,
    ]);

    public static FrozenSet<string> MaterialTypeDescriptions { get; } = FrozenSet.ToFrozenSet(
    [
        SH.ModelMetadataMaterialCharacterAndWeaponEnhancementMaterial,
        SH.ModelMetadataMaterialCharacterEXPMaterial,
        SH.ModelMetadataMaterialCharacterAscensionMaterial,
        SH.ModelMetadataMaterialCharacterTalentMaterial,
        SH.ModelMetadataMaterialCharacterLevelUpMaterial,
        SH.ModelMetadataMaterialWeaponEnhancementMaterial,
        SH.ModelMetadataMaterialWeaponAscensionMaterial,
    ]);
}