// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Package.Advanced.Model;

internal sealed class SophonDecodedBuild
{
    private static readonly Func<AssetProperty, int> SumAssetPropertyAssetChunks = property => property.AssetChunks.Count;
    private static readonly Func<SophonDecodedManifest, int> SumSophonDecodedManifestAssets = manifest => manifest.Data.Assets.Sum(SumAssetPropertyAssetChunks);

    public SophonDecodedBuild(string tag, long downloadTotalBytes, long uncompressedTotalBytes, ImmutableArray<SophonDecodedManifest> manifests)
    {
        Tag = tag;
        DownloadTotalBytes = downloadTotalBytes;
        UncompressedTotalBytes = uncompressedTotalBytes;
        Manifests = manifests;
    }

    public string Tag { get; }

    public long DownloadTotalBytes { get; }

    public long UncompressedTotalBytes { get; }

    /// <summary>
    /// One main package and several audio packages followed up.
    /// </summary>
    public ImmutableArray<SophonDecodedManifest> Manifests { get; }

    public int TotalChunks { get => Manifests.Sum(SumSophonDecodedManifestAssets); }
}