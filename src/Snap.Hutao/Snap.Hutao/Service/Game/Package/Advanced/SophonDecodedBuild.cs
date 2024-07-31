// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class SophonDecodedBuild
{
    public SophonDecodedBuild(long totalBytes, List<SophonDecodedManifest> manifests)
    {
        TotalBytes = totalBytes;
        Manifests = manifests;
    }

    public long TotalBytes { get; }

    public List<SophonDecodedManifest> Manifests { get; }

    public int TotalBlockCount { get => Manifests.Sum(manifest => manifest.ManifestProto.Assets.Sum(a => a.AssetChunks.Count)); }
}