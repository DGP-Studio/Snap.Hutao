// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal static class NameIconDescriptionBuilderExtension
{
    public static TBuilder SetDescription<TBuilder, T>(this TBuilder builder, string description)
        where TBuilder : class, INameIconDescriptionBuilder<T>
        where T : NameIconDescription
    {
        return builder.Configure(b => b.View.Description = description);
    }

    public static TBuilder SetIcon<TBuilder, T>(this TBuilder builder, Uri icon)
        where TBuilder : class, INameIconDescriptionBuilder<T>
        where T : NameIconDescription
    {
        return builder.Configure(b => b.View.Icon = icon);
    }

    public static TBuilder SetName<TBuilder, T>(this TBuilder builder, string name)
        where TBuilder : class, INameIconDescriptionBuilder<T>
        where T : NameIconDescription
    {
        return builder.Configure(b => b.View.Name = name);
    }
}