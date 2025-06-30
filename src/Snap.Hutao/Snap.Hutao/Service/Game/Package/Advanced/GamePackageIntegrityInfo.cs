// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class GamePackageIntegrityInfo
{
    public required ImmutableArray<SophonAssetOperation> ConflictedAssets { get; init; }

    public required bool ChannelSdkConflicted { get; init; }

    public bool NoConflict { get => ConflictedAssets is [] && !ChannelSdkConflicted; }

    public (int ChunkCount, long ByteCount) GetConflictedBlockCountAndByteCount(GameChannelSDK? sdk)
    {
        int conflictChunks = ConflictedAssets.Sum(a => a.NewAsset.AssetChunks.Count);
        long conflictBytes = ConflictedAssets.Sum(a => a.NewAsset.AssetSize);

        if (ChannelSdkConflicted)
        {
            ArgumentNullException.ThrowIfNull(sdk);
            conflictChunks++;
            conflictBytes += sdk.ChannelSdkPackage.Size;
        }

        return (conflictChunks, conflictBytes);
    }
}