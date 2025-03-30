// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal static class WeaponViewBuilderExtension
{
    public static TBuilder SetAffixLevelNumber<TBuilder>(this TBuilder builder, uint affixLevelNumber)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.Configure(b => b.View.AffixLevelNumber = affixLevelNumber);
    }

    public static TBuilder SetAffixDescription<TBuilder>(this TBuilder builder, string? affixDescription)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.Configure(b => b.View.AffixDescription = affixDescription ?? string.Empty);
    }

    public static TBuilder SetAffixName<TBuilder>(this TBuilder builder, string? affixName)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.Configure(b => b.View.AffixName = affixName ?? string.Empty);
    }

    public static TBuilder SetDescription<TBuilder>(this TBuilder builder, string description)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.SetDescription<TBuilder, WeaponView>(description);
    }

    public static TBuilder SetEquipType<TBuilder>(this TBuilder builder, EquipType equipType)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.SetEquipType<TBuilder, WeaponView>(equipType);
    }

    public static TBuilder SetIcon<TBuilder>(this TBuilder builder, Uri icon)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.SetIcon<TBuilder, WeaponView>(icon);
    }

    public static TBuilder SetId<TBuilder>(this TBuilder builder, WeaponId id)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.Configure(b => b.View.Id = id);
    }

    public static TBuilder SetLevel<TBuilder>(this TBuilder builder, string level)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.SetLevel<TBuilder, WeaponView>(level);
    }

    public static TBuilder SetLevelNumber<TBuilder>(this TBuilder builder, uint levelNumber)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.Configure(b => b.View.LevelNumber = levelNumber);
    }

    public static TBuilder SetMainProperty<TBuilder>(this TBuilder builder, NameValue<string>? mainProperty)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.SetMainProperty<TBuilder, WeaponView>(mainProperty);
    }

    public static TBuilder SetName<TBuilder>(this TBuilder builder, string name)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.SetName<TBuilder, WeaponView>(name);
    }

    public static TBuilder SetPromoteLevel<TBuilder>(this TBuilder builder, PromoteLevel promoteLevel)
        where TBuilder : IWeaponViewBuilder
    {
        builder.View.PromoteLevel = promoteLevel;

        bool[] promoteArray = new bool[6];
        promoteArray.AsSpan(0, (int)(uint)promoteLevel).Fill(true);
        builder.View.PromoteArray = ImmutableCollectionsMarshal.AsImmutableArray(promoteArray);

        return builder;
    }

    public static TBuilder SetQuality<TBuilder>(this TBuilder builder, QualityType quality)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.SetQuality<TBuilder, WeaponView>(quality);
    }

    public static TBuilder SetSubProperty<TBuilder>(this TBuilder builder, NameValue<string>? subProperty)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.Configure(b => b.View.SubProperty = subProperty);
    }

    public static TBuilder SetWeaponType<TBuilder>(this TBuilder builder, WeaponType weaponType)
        where TBuilder : class, IWeaponViewBuilder
    {
        return builder.Configure(b => b.View.WeaponType = weaponType);
    }
}