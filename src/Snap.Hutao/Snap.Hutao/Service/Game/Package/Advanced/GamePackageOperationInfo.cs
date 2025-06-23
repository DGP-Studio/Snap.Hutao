// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Package.Advanced.Model;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class GamePackageOperationInfo
{
    public GamePackageOperationInfo(int downloadTotalChunks, int installTotalChunks, long downloadTotalBytes, long installTotalBytes, ImmutableArray<SophonAssetOperation> diffAssetOperations)
    {
        DownloadTotalChunks = downloadTotalChunks;
        InstallTotalChunks = installTotalChunks;
        DownloadTotalBytes = downloadTotalBytes;
        InstallTotalBytes = installTotalBytes;
        DiffAssetOperations = diffAssetOperations;
    }

    public int DownloadTotalChunks { get; }

    public int InstallTotalChunks { get; }

    public long DownloadTotalBytes { get; }

    public long InstallTotalBytes { get; }

    public ImmutableArray<SophonAssetOperation> DiffAssetOperations { get; }
}