// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class GamePackageServiceOperationInformationTraits
{
    private readonly IContentDialogFactory contentDialogFactory;

    [GeneratedConstructor]
    public partial GamePackageServiceOperationInformationTraits(IServiceProvider serviceProvider);

    public async ValueTask<GamePackageOperationInfo?> EnsureAvailableFreeSpaceAndPrepareAsync(GamePackageOperationContext context)
    {
        SophonDecodedBuild? localBuild = context.LocalBuild;
        SophonDecodedBuild? remoteBuild = context.RemoteBuild;

        // LocalBuild can be null when perform install operation
        ArgumentNullException.ThrowIfNull(remoteBuild);

        if (context.Kind is GamePackageOperationKind.Verify)
        {
            ArgumentNullException.ThrowIfNull(localBuild);
            return new(0, localBuild.TotalChunks, 0, localBuild.UncompressedTotalBytes, default);
        }

        ImmutableArray<SophonAssetOperation> diffAssets = context.Kind switch
        {
            GamePackageOperationKind.Install or GamePackageOperationKind.ExtractExecutable => default,
            GamePackageOperationKind.Update or GamePackageOperationKind.Predownload or GamePackageOperationKind.ExtractBlocks => [.. GetDiffOperations(localBuild, remoteBuild).OrderBy(a => a.Kind)],
            _ => throw HutaoException.NotSupported(),
        };

        (long downloadTotalBytes, long installTotalBytes) = context.Kind switch
        {
            GamePackageOperationKind.Install or GamePackageOperationKind.ExtractExecutable => (remoteBuild.DownloadTotalBytes, remoteBuild.UncompressedTotalBytes),
            GamePackageOperationKind.Update or GamePackageOperationKind.Predownload or GamePackageOperationKind.ExtractBlocks => GetTotalByteGroupsWithPatchBuild(context, diffAssets),
            _ => throw HutaoException.NotSupported(),
        };

        long downloadedTotalBytes = GetSize(context.EffectiveChunksDirectory);
        long actualTotalBytes = installTotalBytes - downloadedTotalBytes + (1024L * 1024L * 1024L); // 1 GB for temp files
        long availableBytes = LogicalDrive.GetAvailableFreeSpace(context.EffectiveGameDirectory);

        string formattedDownloadTotalBytes = Converters.ToFileSizeString(downloadTotalBytes);
        string formattedTotalBytes = Converters.ToFileSizeString(actualTotalBytes);
        string formattedAvailableBytes = Converters.ToFileSizeString(availableBytes);

        bool hasAvailableFreeSpace = actualTotalBytes <= availableBytes;

        string title = GetDialogTitle(context, hasAvailableFreeSpace);
        string message = SH.FormatServiceGamePackageAdvancedConfirmMessage(formattedDownloadTotalBytes, formattedTotalBytes, formattedAvailableBytes);

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(title, message, ContentDialogButton.Primary, hasAvailableFreeSpace)
            .ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return default;
        }

        (int downloadTotalBlocks, int installTotalBlocks) = context.Kind switch
        {
            GamePackageOperationKind.Install or GamePackageOperationKind.ExtractExecutable => (remoteBuild.TotalChunks, remoteBuild.TotalChunks),
            GamePackageOperationKind.Update or GamePackageOperationKind.Predownload or GamePackageOperationKind.ExtractBlocks => GetTotalBlockGroupsWithPatchBuild(context, diffAssets),
            _ => throw HutaoException.NotSupported(),
        };

        return new(downloadTotalBlocks, installTotalBlocks, downloadTotalBytes, installTotalBytes, diffAssets);
    }

    private static (long DownloadTotalBytes, long InstallTotalBytes) GetTotalByteGroupsWithPatchBuild(GamePackageOperationContext context, ImmutableArray<SophonAssetOperation> assets)
    {
        if (context.PatchBuild is { } patchBuild)
        {
            return (patchBuild.DownloadTotalBytes, patchBuild.UncompressedTotalBytes);
        }

        return context.Kind switch
        {
            GamePackageOperationKind.Update or GamePackageOperationKind.Predownload => (GetDownloadTotalBytes(assets), GetUnCompressedTotalBytes(assets)),
            GamePackageOperationKind.ExtractBlocks => (GetUnCompressedTotalBytes(assets, true), GetUnCompressedTotalBytes(assets, true)),
            _ => throw HutaoException.NotSupported(),
        };
    }

    private static (int DownloadTotalBlocks, int InstallTotalBlocks) GetTotalBlockGroupsWithPatchBuild(GamePackageOperationContext context, ImmutableArray<SophonAssetOperation> assets)
    {
        if (context.PatchBuild is { } patchBuild)
        {
            return ((int)patchBuild.DownloadFileCount, (int)patchBuild.InstallFileCount);
        }

        return context.Kind switch
        {
            GamePackageOperationKind.Update or GamePackageOperationKind.ExtractBlocks => (GetDownloadTotalBlocks(assets), GetInstallTotalBlocks(assets)),
            GamePackageOperationKind.Predownload => (GetDownloadTotalBlocks(assets), 0),
            _ => throw HutaoException.NotSupported(),
        };
    }

    private static string GetDialogTitle(GamePackageOperationContext context, bool hasAvailableFreeSpace)
    {
        if (!hasAvailableFreeSpace)
        {
            return SH.ServiceGamePackageAdvancedDriverNoAvailableFreeSpace;
        }

        return context.Kind switch
        {
            GamePackageOperationKind.Install => SH.ServiceGamePackageAdvancedConfirmStartInstallTitle,
            GamePackageOperationKind.Update => SH.ServiceGamePackageAdvancedConfirmStartUpdateTitle,
            GamePackageOperationKind.Predownload => SH.ServiceGamePackageAdvancedConfirmStartPredownloadTitle,
            GamePackageOperationKind.ExtractBlocks => "Start extracting game blocks?",
            GamePackageOperationKind.ExtractExecutable => "Start extracting game executable?",
            _ => throw HutaoException.NotSupported(),
        };
    }

    private static IEnumerable<SophonAssetOperation> GetDiffOperations(SophonDecodedBuild? localDecodedBuild, SophonDecodedBuild remoteDecodedBuild)
    {
        ArgumentNullException.ThrowIfNull(localDecodedBuild);
        foreach ((SophonDecodedManifest localManifest, SophonDecodedManifest remoteManifest) in localDecodedBuild.Manifests.Zip(remoteDecodedBuild.Manifests))
        {
            foreach (AssetProperty remoteAsset in remoteManifest.Data.Assets)
            {
                if (localManifest.Data.Assets.FirstOrDefault(localAsset => localAsset.AssetName.Equals(remoteAsset.AssetName, StringComparison.OrdinalIgnoreCase)) is not { } localAsset)
                {
                    yield return SophonAssetOperation.AddOrRepair(remoteManifest.UrlPrefix, remoteManifest.UrlSuffix, remoteAsset);
                    continue;
                }

                if (localAsset.AssetHashMd5.Equals(remoteAsset.AssetHashMd5, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                ImmutableArray<SophonChunk>.Builder diffChunks = ImmutableArray.CreateBuilder<SophonChunk>();
                foreach (AssetChunk chunk in remoteAsset.AssetChunks)
                {
                    if (localAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5.Equals(chunk.ChunkDecompressedHashMd5, StringComparison.OrdinalIgnoreCase)) is null)
                    {
                        diffChunks.Add(new(remoteManifest.UrlPrefix, remoteManifest.UrlSuffix, chunk));
                    }
                }

                yield return SophonAssetOperation.Modify(remoteManifest.UrlPrefix, remoteManifest.UrlSuffix, localAsset, remoteAsset, diffChunks.ToImmutable());
            }

            foreach (AssetProperty localAsset in localManifest.Data.Assets)
            {
                if (remoteManifest.Data.Assets.FirstOrDefault(a => a.AssetName.Equals(localAsset.AssetName, StringComparison.OrdinalIgnoreCase)) is null)
                {
                    yield return SophonAssetOperation.Delete(localAsset);
                }
            }
        }
    }

    // TODO: Cache these SUM results in SophonAssetOperation
    private static long GetDownloadTotalBytes(ImmutableArray<SophonAssetOperation> assets)
    {
        long downloadTotalBytes = 0;
        foreach (ref readonly SophonAssetOperation assetOperation in assets.AsSpan())
        {
            switch (assetOperation.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair:
                    downloadTotalBytes += assetOperation.NewAsset.AssetChunks.Sum(c => c.ChunkSize);
                    break;
                case SophonAssetOperationKind.Modify:
                    downloadTotalBytes += assetOperation.DiffChunks.Sum(c => c.AssetChunk.ChunkSize);
                    break;
            }
        }

        return downloadTotalBytes;
    }

    private static long GetUnCompressedTotalBytes(ImmutableArray<SophonAssetOperation> assets, bool isExtractBlk = false)
    {
        long totalBytes = 0;
        foreach (ref readonly SophonAssetOperation diffAsset in assets.AsSpan())
        {
            switch (diffAsset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair:
                    totalBytes += diffAsset.NewAsset.AssetSize;
                    break;
                case SophonAssetOperationKind.Modify:
                    totalBytes += isExtractBlk ? diffAsset.NewAsset.AssetSize : diffAsset.DiffChunks.Sum(c => c.AssetChunk.ChunkSizeDecompressed);
                    break;
            }
        }

        return totalBytes;
    }

    private static int GetDownloadTotalBlocks(ImmutableArray<SophonAssetOperation> assets)
    {
        int totalBlocks = 0;
        foreach (ref readonly SophonAssetOperation asset in assets.AsSpan())
        {
            switch (asset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair:
                    totalBlocks += asset.NewAsset.AssetChunks.Count;
                    break;
                case SophonAssetOperationKind.Modify:
                    totalBlocks += asset.DiffChunks.Length;
                    break;
            }
        }

        return totalBlocks;
    }

    private static int GetInstallTotalBlocks(ImmutableArray<SophonAssetOperation> assets)
    {
        int totalBlocks = 0;
        foreach (ref readonly SophonAssetOperation asset in assets.AsSpan())
        {
            switch (asset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair or SophonAssetOperationKind.Modify:
                    totalBlocks += asset.NewAsset.AssetChunks.Count;
                    break;
            }
        }

        return totalBlocks;
    }

    private static long GetSize(string path)
    {
        if (!Directory.Exists(path))
        {
            return 0;
        }

        long size = 0;
        DateTime cutoffDate = DateTime.Now.AddDays(-5);

        try
        {
            foreach (string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    System.IO.FileInfo fileInfo = new(file);
                    if (fileInfo.CreationTime <= cutoffDate)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception)
                        {
                            // Ignore
                        }

                        continue;
                    }

                    size += fileInfo.Length;
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }
        catch (Exception)
        {
            return 0;
        }

        return size;
    }
}