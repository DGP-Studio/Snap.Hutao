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
    public static FrozenSet<string> AssociationTypes { get; } = Enum.GetValues<AssociationType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToFrozenSet();

    public static FrozenSet<NameValue<AssociationType>> AssociationTypeNameValues { get; } = Enum.GetValues<AssociationType>().Select(e => new NameValue<AssociationType>(e.GetLocalizedDescriptionOrDefault()!, e)).Where(nv => !string.IsNullOrEmpty(nv.Name)).ToFrozenSet();

    public static FrozenSet<string> WeaponTypes { get; } = Enum.GetValues<WeaponType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToFrozenSet();

    public static FrozenSet<NameValue<WeaponType>> WeaponTypeNameValues { get; } = Enum.GetValues<WeaponType>().Select(e => new NameValue<WeaponType>(e.GetLocalizedDescriptionOrDefault()!, e)).Where(nv => !string.IsNullOrEmpty(nv.Name)).ToFrozenSet();

    public static FrozenSet<string> ItemQualities { get; } = Enum.GetValues<QualityType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToFrozenSet();

    public static FrozenSet<NameValue<QualityType>> ItemQualityNameValues { get; } = Enum.GetValues<QualityType>().Select(e => new NameValue<QualityType>(e.GetLocalizedDescriptionOrDefault()!, e)).Where(nv => !string.IsNullOrEmpty(nv.Name)).ToFrozenSet();

    public static FrozenSet<string> BodyTypes { get; } = Enum.GetValues<BodyType>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToFrozenSet();

    public static FrozenSet<NameValue<BodyType>> BodyTypeNameValues { get; } = Enum.GetValues<BodyType>().Select(e => new NameValue<BodyType>(e.GetLocalizedDescriptionOrDefault()!, e)).Where(nv => !string.IsNullOrEmpty(nv.Name)).ToFrozenSet();

    public static FrozenSet<string> FightProperties { get; } = Enum.GetValues<FightProperty>().Select(e => e.GetLocalizedDescriptionOrDefault()).OfType<string>().ToFrozenSet();

    public static FrozenSet<NameValue<FightProperty>> FightPropertyNameValues { get; } = Enum.GetValues<FightProperty>().Select(e => new NameValue<FightProperty>(e.GetLocalizedDescriptionOrDefault()!, e)).Where(nv => !string.IsNullOrEmpty(nv.Name)).ToFrozenSet();

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

    public static FrozenSet<NameValue<int>> ElementNameValues { get; } = FrozenSet.ToFrozenSet(
    [
        new NameValue<int>(SH.ModelIntrinsicElementNameFire, 1),
        new NameValue<int>(SH.ModelIntrinsicElementNameWater, 2),
        new NameValue<int>(SH.ModelIntrinsicElementNameGrass, 3),
        new NameValue<int>(SH.ModelIntrinsicElementNameElec, 4),
        new NameValue<int>(SH.ModelIntrinsicElementNameWind, 5),
        new NameValue<int>(SH.ModelIntrinsicElementNameIce, 6),
        new NameValue<int>(SH.ModelIntrinsicElementNameRock, 7),
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