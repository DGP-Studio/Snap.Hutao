// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package;

internal sealed class AssetChunkMd5Comparer : IEqualityComparer<AssetChunk>
{
    private static AssetChunkMd5Comparer? shared;
    private static object? syncRoot;

    public static AssetChunkMd5Comparer Shared { get => LazyInitializer.EnsureInitialized(ref shared, ref syncRoot, () => new()); }

    public bool Equals(AssetChunk? x, AssetChunk? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        return string.Equals(x.ChunkDecompressedHashMd5, y.ChunkDecompressedHashMd5, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode([DisallowNull] AssetChunk obj)
    {
        return obj.GetHashCode();
    }

}