// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package;

internal sealed class SophonDecodedBuild
{
    public SophonDecodedBuild(long totalBytes, List<SophonDecodedManifest> manifests)
    {
        TotalBytes = totalBytes;
        Manifests = manifests;
    }

    public long TotalBytes { get; }

    public List<SophonDecodedManifest> Manifests { get; }
}