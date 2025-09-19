// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Property;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game;

internal static class AspectRatioExtension
{
    public static ImmutableArray<AspectRatio> Add(this IProperty<ImmutableArray<AspectRatio>> aspectRatios, AspectRatio aspectRatio)
    {
        if (!aspectRatios.Value.Contains(aspectRatio))
        {
            aspectRatios.Value = aspectRatios.Value.Add(aspectRatio);
        }

        return aspectRatios.Value;
    }

    public static ImmutableArray<AspectRatio> Remove(this IProperty<ImmutableArray<AspectRatio>> aspectRatios, AspectRatio aspectRatio)
    {
        return aspectRatios.Value = aspectRatios.Value.Remove(aspectRatio);
    }
}