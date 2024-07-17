// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal static class ReliquaryViewBuilderExtension
{
    public static TBuilder SetComposedSubProperties<TBuilder>(this TBuilder builder, List<ReliquaryComposedSubProperty> composedSubProperties)
        where TBuilder : IReliquaryViewBuilder
    {
        return builder.Configure(b => b.View.ComposedSubProperties = composedSubProperties);
    }

    public static TBuilder SetDescription<TBuilder>(this TBuilder builder, string description)
        where TBuilder : IReliquaryViewBuilder
    {
        return builder.SetDescription<TBuilder, ReliquaryView>(description);
    }

    public static TBuilder SetEquipType<TBuilder>(this TBuilder builder, EquipType equipType)
        where TBuilder : IReliquaryViewBuilder
    {
        return builder.SetEquipType<TBuilder, ReliquaryView>(equipType);
    }

    public static TBuilder SetIcon<TBuilder>(this TBuilder builder, Uri icon)
        where TBuilder : IReliquaryViewBuilder
    {
        return builder.SetIcon<TBuilder, ReliquaryView>(icon);
    }

    public static TBuilder SetLevel<TBuilder>(this TBuilder builder, string level)
        where TBuilder : IReliquaryViewBuilder
    {
        return builder.SetLevel<TBuilder, ReliquaryView>(level);
    }

    public static TBuilder SetMainProperty<TBuilder>(this TBuilder builder, NameValue<string> mainProperty)
        where TBuilder : IReliquaryViewBuilder
    {
        return builder.SetMainProperty<TBuilder, ReliquaryView>(mainProperty);
    }

    public static TBuilder SetName<TBuilder>(this TBuilder builder, string name)
        where TBuilder : IReliquaryViewBuilder
    {
        return builder.SetName<TBuilder, ReliquaryView>(name);
    }

    public static TBuilder SetPrimarySubProperties<TBuilder>(this TBuilder builder, List<ReliquarySubProperty> primarySubProperties)
        where TBuilder : IReliquaryViewBuilder
    {
        return builder.Configure(b => b.View.PrimarySubProperties = primarySubProperties);
    }

    public static TBuilder SetQuality<TBuilder>(this TBuilder builder, QualityType quality)
        where TBuilder : IReliquaryViewBuilder
    {
        return builder.SetQuality<TBuilder, ReliquaryView>(quality);
    }

    public static TBuilder SetSecondarySubProperties<TBuilder>(this TBuilder builder, List<ReliquarySubProperty> secondarySubProperties)
        where TBuilder : IReliquaryViewBuilder
    {
        return builder.Configure(b => b.View.SecondarySubProperties = secondarySubProperties);
    }

    public static TBuilder SetSetName<TBuilder>(this TBuilder builder, string setName)
        where TBuilder : IReliquaryViewBuilder
    {
        return builder.Configure(b => b.View.SetName = setName);
    }
}