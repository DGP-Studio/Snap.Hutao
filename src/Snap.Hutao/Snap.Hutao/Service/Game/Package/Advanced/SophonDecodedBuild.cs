// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class SophonDecodedBuild
{
    private static readonly Func<AssetProperty, int> SumAssetPropertyAssetChunks = property => property.AssetChunks.Count;
    private static readonly Func<SophonDecodedManifest, int> SumSophonDecodedManifestAssets = manifest => manifest.ManifestProto.Assets.Sum(SumAssetPropertyAssetChunks);

    public SophonDecodedBuild(long totalBytes, List<SophonDecodedManifest> manifests)
    {
        TotalBytes = totalBytes;
        Manifests = manifests;
    }

    public long TotalBytes { get; }

    public List<SophonDecodedManifest> Manifests { get; }

    public int TotalChunks { get => Manifests.Sum(SumSophonDecodedManifestAssets); }
}