// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package;

internal sealed class AssetPropertyNameComparer : IEqualityComparer<AssetProperty>
{
    private static AssetPropertyNameComparer? shared;
    private static object? syncRoot;

    public static AssetPropertyNameComparer Shared { get => LazyInitializer.EnsureInitialized(ref shared, ref syncRoot, () => new()); }

    public bool Equals(AssetProperty? x, AssetProperty? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        return string.Equals(x.AssetName, y.AssetName, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode([DisallowNull] AssetProperty obj)
    {
        return HashCode.Combine(obj.AssetName);
    }
}