﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class GamePackageIntegrityInfo
{
    public required List<SophonAssetOperation> ConflictedAssets { get; init; }

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