// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.ViewModel.AvatarProperty;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal static class ReliquaryViewBuilderExtension
{
    public static TBuilder SetComposedSubProperties<TBuilder>(this TBuilder builder, ImmutableArray<ReliquaryComposedSubProperty> composedSubProperties)
        where TBuilder : class, IReliquaryViewBuilder
    {
        return builder.Configure(b => b.View.ComposedSubProperties = composedSubProperties);
    }

    public static TBuilder SetDescription<TBuilder>(this TBuilder builder, string description)
        where TBuilder : class, IReliquaryViewBuilder
    {
        return builder.SetDescription<TBuilder, ReliquaryView>(description);
    }

    public static TBuilder SetEquipType<TBuilder>(this TBuilder builder, EquipType equipType)
        where TBuilder : class, IReliquaryViewBuilder
    {
        return builder.SetEquipType<TBuilder, ReliquaryView>(equipType);
    }

    public static TBuilder SetIcon<TBuilder>(this TBuilder builder, Uri icon)
        where TBuilder : class, IReliquaryViewBuilder
    {
        return builder.SetIcon<TBuilder, ReliquaryView>(icon);
    }

    public static TBuilder SetLevel<TBuilder>(this TBuilder builder, string level)
        where TBuilder : class, IReliquaryViewBuilder
    {
        return builder.SetLevel<TBuilder, ReliquaryView>(level);
    }

    public static TBuilder SetMainProperty<TBuilder>(this TBuilder builder, NameValue<string> mainProperty)
        where TBuilder : class, IReliquaryViewBuilder
    {
        return builder.SetMainProperty<TBuilder, ReliquaryView>(mainProperty);
    }

    public static TBuilder SetName<TBuilder>(this TBuilder builder, string name)
        where TBuilder : class, IReliquaryViewBuilder
    {
        return builder.SetName<TBuilder, ReliquaryView>(name);
    }

    public static TBuilder SetQuality<TBuilder>(this TBuilder builder, QualityType quality)
        where TBuilder : class, IReliquaryViewBuilder
    {
        return builder.SetQuality<TBuilder, ReliquaryView>(quality);
    }

    public static TBuilder SetSetName<TBuilder>(this TBuilder builder, string setName)
        where TBuilder : class, IReliquaryViewBuilder
    {
        return builder.Configure(b => b.View.SetName = setName);
    }
}