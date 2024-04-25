// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction.Extension;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal static class EquipViewBuilderExtension
{
    public static TBuilder SetLevel<TBuilder, T>(this TBuilder builder, string level)
        where TBuilder : IEquipViewBuilder<T>
        where T : EquipView
    {
        return builder.Configure(b => b.View.Level = level);
    }

    public static TBuilder SetQuality<TBuilder, T>(this TBuilder builder, QualityType quality)
        where TBuilder : IEquipViewBuilder<T>
        where T : EquipView
    {
        return builder.Configure(b => b.View.Quality = quality);
    }

    public static TBuilder SetMainProperty<TBuilder, T>(this TBuilder builder, NameValue<string> mainProperty)
        where TBuilder : IEquipViewBuilder<T>
        where T : EquipView
    {
        return builder.Configure(b => b.View.MainProperty = mainProperty);
    }
}