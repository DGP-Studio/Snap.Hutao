// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Package.Advanced.Model;

internal sealed class SophonDecodedPatchBuild
{
    public SophonDecodedPatchBuild(string originalTag, string tag, long downloadTotalBytes, long downloadFileCount, long uncompressedTotalBytes, long installFileCount, ImmutableArray<SophonDecodedPatchManifest> manifests)
    {
        OriginalTag = originalTag;
        Tag = tag;
        DownloadTotalBytes = downloadTotalBytes;
        DownloadFileCount = downloadFileCount;
        UncompressedTotalBytes = uncompressedTotalBytes;
        InstallFileCount = installFileCount;
        Manifests = manifests;
    }

    public string OriginalTag { get; }

    public string Tag { get; }

    public long DownloadTotalBytes { get; }

    public long DownloadFileCount { get; }

    public long UncompressedTotalBytes { get; }

    public long InstallFileCount { get; }

    /// <summary>
    /// One main package and several audio packages followed up.
    /// </summary>
    public ImmutableArray<SophonDecodedPatchManifest> Manifests { get; }
}