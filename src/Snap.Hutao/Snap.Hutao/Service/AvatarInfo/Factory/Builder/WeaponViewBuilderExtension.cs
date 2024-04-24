// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction.Extension;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Enka.Model;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal static class WeaponViewBuilderExtension
{
    public static TBuilder SetAffixLevelNumber<TBuilder>(this TBuilder builder, uint affixLevelNumber)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.AffixLevelNumber = affixLevelNumber);
    }

    public static TBuilder SetAffixDescription<TBuilder>(this TBuilder builder, string? affixDescription)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.AffixDescription = affixDescription ?? string.Empty);
    }

    public static TBuilder SetAffixName<TBuilder>(this TBuilder builder, string affixName)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.AffixName = affixName);
    }

    public static TBuilder SetDescription<TBuilder>(this TBuilder builder, string description)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.Description = description);
    }

    public static TBuilder SetIcon<TBuilder>(this TBuilder builder, Uri icon)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.Icon = icon);
    }

    public static TBuilder SetId<TBuilder>(this TBuilder builder, WeaponId id)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.Id = id);
    }

    public static TBuilder SetLevel<TBuilder>(this TBuilder builder, string level)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.Level = level);
    }

    public static TBuilder SetLevelNumber<TBuilder>(this TBuilder builder, uint levelNumber)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.LevelNumber = levelNumber);
    }

    public static TBuilder SetMainProperty<TBuilder>(this TBuilder builder, WeaponStat? mainStat)
    where TBuilder : IWeaponViewBuilder
    {
        return builder.SetMainProperty(mainStat is not null ? FightPropertyFormat.ToNameValue(mainStat.AppendPropId, mainStat.StatValue) : NameValueDefaults.String);
    }

    public static TBuilder SetMainProperty<TBuilder>(this TBuilder builder, NameValue<string> mainProperty)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.MainProperty = mainProperty);
    }

    public static TBuilder SetName<TBuilder>(this TBuilder builder, string name)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.Name = name);
    }

    public static TBuilder SetQuality<TBuilder>(this TBuilder builder, QualityType quality)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.Quality = quality);
    }

    public static TBuilder SetSubProperty<TBuilder>(this TBuilder builder, NameDescription subProperty)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.SubProperty = subProperty);
    }

    public static TBuilder SetWeaponType<TBuilder>(this TBuilder builder, WeaponType weaponType)
        where TBuilder : IWeaponViewBuilder
    {
        return builder.Configure(b => b.WeaponView.WeaponType = weaponType);
    }
}