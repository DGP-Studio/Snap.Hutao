// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.IO;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using Snap.Hutao.Web.Response;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace Snap.Hutao.Service.Game.Package;

internal sealed class GamePackageIntegrityInfo
{
    public required List<SophonAsset> ConflictedAssets { get; init; }

    public required bool ChannelSdkConflicted { get; init; }

    public bool NoConflict { get => ConflictedAssets is [] && !ChannelSdkConflicted; }

    public (int BlockCount, long ByteCount) GetConflictedBlockCountAndByteCount(GameChannelSDK? sdk)
    {
        int conflictBlocks = ConflictedAssets.Sum(a => a.AssetProperty.AssetChunks.Count);
        long conflictBytes = ConflictedAssets.Sum(a => a.AssetProperty.AssetSize);

        if (ChannelSdkConflicted)
        {
            ArgumentNullException.ThrowIfNull(sdk);
            conflictBlocks++;
            conflictBytes += sdk.ChannelSdkPackage.Size;
        }

        return (conflictBlocks, conflictBytes);
    }
}