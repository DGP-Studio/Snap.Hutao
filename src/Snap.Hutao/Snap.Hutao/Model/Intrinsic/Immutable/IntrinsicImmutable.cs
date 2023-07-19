// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Intrinsic.Immutable;

/// <summary>
/// 本地化的不可变的原生枚举
/// </summary>
[HighQuality]
internal static class IntrinsicImmutable
{
    /// <summary>
    /// 所属地区
    /// </summary>
    public static readonly ImmutableHashSet<string> AssociationTypes = Enum.GetValues<AssociationType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToImmutableHashSet();

    /// <summary>
    /// 武器类型
    /// </summary>
    public static readonly ImmutableHashSet<string> WeaponTypes = Enum.GetValues<WeaponType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToImmutableHashSet();

    /// <summary>
    /// 物品类型
    /// </summary>
    public static readonly ImmutableHashSet<string> ItemQualities = Enum.GetValues<QualityType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToImmutableHashSet();

    /// <summary>
    /// 身材类型
    /// </summary>
    public static readonly ImmutableHashSet<string> BodyTypes = Enum.GetValues<BodyType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToImmutableHashSet();

    /// <summary>
    /// 战斗属性
    /// </summary>
    public static readonly ImmutableHashSet<string> FightProperties = Enum.GetValues<FightProperty>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToImmutableHashSet();

    /// <summary>
    /// 元素名称
    /// </summary>
    public static readonly ImmutableHashSet<string> ElementNames = new HashSet<string>(7)
    {
        SH.ModelIntrinsicElementNameFire,
        SH.ModelIntrinsicElementNameWater,
        SH.ModelIntrinsicElementNameGrass,
        SH.ModelIntrinsicElementNameElec,
        SH.ModelIntrinsicElementNameWind,
        SH.ModelIntrinsicElementNameIce,
        SH.ModelIntrinsicElementNameRock,
    }.ToImmutableHashSet();

    public static readonly ImmutableHashSet<string> MaterialTypeDescriptions = new HashSet<string>(7)
    {
        SH.ModelMetadataMaterialCharacterAndWeaponEnhancementMaterial,
        SH.ModelMetadataMaterialCharacterEXPMaterial,
        SH.ModelMetadataMaterialCharacterAscensionMaterial,
        SH.ModelMetadataMaterialCharacterTalentMaterial,
        SH.ModelMetadataMaterialCharacterLevelUpMaterial,
        SH.ModelMetadataMaterialWeaponEnhancementMaterial,
        SH.ModelMetadataMaterialWeaponAscensionMaterial,
    }.ToImmutableHashSet();
}