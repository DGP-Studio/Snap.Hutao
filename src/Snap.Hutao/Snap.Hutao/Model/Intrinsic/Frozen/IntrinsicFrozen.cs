// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity.Primitive;
using System.Collections.Frozen;
using System.Globalization;

namespace Snap.Hutao.Model.Intrinsic.Frozen;

internal static class IntrinsicFrozen
{
    public static FrozenSet<string> AssociationTypes { get; } = NamesFromEnum<AssociationType>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<NameValue<AssociationType>> AssociationTypeNameValues { get; } = NameValuesFromEnum<AssociationType>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<string> WeaponTypes { get; } = NamesFromEnum<WeaponType>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<NameValue<WeaponType>> WeaponTypeNameValues { get; } = NameValuesFromEnum<WeaponType>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<string> ItemQualities { get; } = NamesFromEnum<QualityType>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<NameValue<QualityType>> ItemQualityNameValues { get; } = NameValuesFromEnum<QualityType>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<string> BodyTypes { get; } = NamesFromEnum<BodyType>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<NameValue<BodyType>> BodyTypeNameValues { get; } = NameValuesFromEnum<BodyType>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<string> FightProperties { get; } = NamesFromEnum<FightProperty>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<NameValue<FightProperty>> FightPropertyNameValues { get; } = NameValuesFromEnum<FightProperty>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<string> CultivateTypes { get; } = NamesFromEnum<CultivateType>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<NameValue<CultivateType>> CultivateTypeNameValues { get; } = NameValuesFromEnum<CultivateType>(e => e.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture));

    public static FrozenSet<string> ElementNames { get; } =
    [
        SH.ModelIntrinsicElementNameFire,
        SH.ModelIntrinsicElementNameWater,
        SH.ModelIntrinsicElementNameGrass,
        SH.ModelIntrinsicElementNameElec,
        SH.ModelIntrinsicElementNameWind,
        SH.ModelIntrinsicElementNameIce,
        SH.ModelIntrinsicElementNameRock,
    ];

    public static FrozenSet<NameValue<int>> ElementNameValues { get; } =
    [
        new(SH.ModelIntrinsicElementNameFire, 1),
        new(SH.ModelIntrinsicElementNameWater, 2),
        new(SH.ModelIntrinsicElementNameGrass, 3),
        new(SH.ModelIntrinsicElementNameElec, 4),
        new(SH.ModelIntrinsicElementNameWind, 5),
        new(SH.ModelIntrinsicElementNameIce, 6),
        new(SH.ModelIntrinsicElementNameRock, 7),
    ];

    public static FrozenSet<string> MaterialTypeDescriptions { get; } =
    [
        SH.ModelMetadataMaterialCharacterAndWeaponEnhancementMaterial,
        SH.ModelMetadataMaterialCharacterEXPMaterial,
        SH.ModelMetadataMaterialCharacterAscensionMaterial,
        SH.ModelMetadataMaterialCharacterTalentMaterial,
        SH.ModelMetadataMaterialCharacterLevelUpMaterial,
        SH.ModelMetadataMaterialWeaponEnhancementMaterial,
        SH.ModelMetadataMaterialWeaponAscensionMaterial,
    ];

    public static FrozenSet<string> ResinMaterialTypeDescriptions { get; } =
    [
        SH.ModelMetadataMaterialCharacterTalentMaterial,
        SH.ModelMetadataMaterialCharacterLevelUpMaterial,
        SH.ModelMetadataMaterialWeaponAscensionMaterial,
    ];

    private static FrozenSet<string> NamesFromEnum<TEnum>(Func<TEnum, string?> selector)
        where TEnum : struct, Enum
    {
        return NamesFromEnumValues(Enum.GetValues<TEnum>(), selector);
    }

    private static FrozenSet<string> NamesFromEnumValues<TEnum>(TEnum[] values, Func<TEnum, string?> selector)
    {
        return FromEnumValues(values, selector, (name, _) => name);
    }

    private static FrozenSet<NameValue<TEnum>> NameValuesFromEnum<TEnum>(Func<TEnum, string?> selector)
        where TEnum : struct, Enum
    {
        return NameValuesFromEnumValues(Enum.GetValues<TEnum>(), selector);
    }

    private static FrozenSet<NameValue<TEnum>> NameValuesFromEnumValues<TEnum>(TEnum[] values, Func<TEnum, string?> selector)
    {
        return FromEnumValues(values, selector, (name, value) => new NameValue<TEnum>(name, value));
    }

    private static FrozenSet<T> FromEnumValues<TEnum, T>(TEnum[] values, Func<TEnum, string?> nameSelector, Func<string, TEnum, T> selector)
    {
        return [.. NotNull(values, nameSelector, selector)];

        static IEnumerable<T> NotNull(TEnum[] values, Func<TEnum, string?> nameSelector, Func<string, TEnum, T> selector)
        {
            foreach (TEnum value in values)
            {
                string? name = nameSelector(value);
                if (!string.IsNullOrEmpty(name))
                {
                    yield return selector(name, value);
                }
            }
        }
    }
}