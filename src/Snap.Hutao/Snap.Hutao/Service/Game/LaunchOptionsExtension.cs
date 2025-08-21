// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Abstraction.Property;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game;

internal static class LaunchOptionsExtension
{
    public static ImmutableArray<AspectRatio> Add(this DbProperty<ImmutableArray<AspectRatio>> aspectRatios, AspectRatio aspectRatio)
    {
        if (!aspectRatios.Value.Contains(aspectRatio))
        {
            aspectRatios.Value = aspectRatios.Value.Add(aspectRatio);
        }

        return aspectRatios.Value;
    }

    public static ImmutableArray<AspectRatio> Remove(this DbProperty<ImmutableArray<AspectRatio>> aspectRatios, AspectRatio aspectRatio)
    {
        aspectRatios.Value = aspectRatios.Value.Remove(aspectRatio);
        return aspectRatios.Value;
    }

    public static NameValue<PlatformType>? GetCurrentPlatformTypeForSelectionOrDefault(this LaunchOptions options)
    {
        return options.PlatformTypes.SingleOrDefault(c => c.Value == options.PlatformType);
    }
}