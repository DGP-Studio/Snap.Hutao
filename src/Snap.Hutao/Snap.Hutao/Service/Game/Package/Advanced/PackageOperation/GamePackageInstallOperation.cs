// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Package.Advanced.Model;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced.PackageOperation;

[Service(ServiceLifetime.Transient, typeof(IGamePackageOperation), Key = GamePackageOperationKind.Install)]
internal sealed class GamePackageInstallOperation : GamePackageOperation
{
    public override async ValueTask ExecuteAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild? remoteBuild = context.Operation.RemoteBuild;
        ArgumentNullException.ThrowIfNull(remoteBuild);
        int totalChunksCount = remoteBuild.TotalChunks;
        long totalBytes = remoteBuild.UncompressedTotalBytes;

        InitializeDuplicatedChunkNames(context, remoteBuild.Manifests.SelectMany(m => m.Data.Assets.SelectMany(a => a.AssetChunks)));

        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedInstalling, totalChunksCount, totalBytes));

        await context.Operation.Asset.InstallAssetsAsync(context, remoteBuild).ConfigureAwait(false);
        await context.Operation.Asset.EnsureChannelSdkAsync(context).ConfigureAwait(false);

        await PrivateVerifyAndRepairAsync(context, remoteBuild, totalBytes, totalChunksCount).ConfigureAwait(false);

        if (Directory.Exists(context.Operation.EffectiveChunksDirectory))
        {
            Directory.Delete(context.Operation.EffectiveChunksDirectory, true);
        }
    }
}