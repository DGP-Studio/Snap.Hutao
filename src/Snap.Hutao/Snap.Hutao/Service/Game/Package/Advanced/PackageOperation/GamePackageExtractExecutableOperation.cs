// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Package.Advanced.Model;

namespace Snap.Hutao.Service.Game.Package.Advanced.PackageOperation;

[Service(ServiceLifetime.Transient, typeof(IGamePackageOperation), Key = GamePackageOperationKind.ExtractExecutable)]
internal sealed class GamePackageExtractExecutableOperation : GamePackageOperation
{
    public override async ValueTask ExecuteAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild? remoteBuild = context.Operation.RemoteBuild;
        ArgumentNullException.ThrowIfNull(remoteBuild);
        int totalChunks = remoteBuild.TotalChunks;
        long downloadTotalBytes = remoteBuild.DownloadTotalBytes;
        long installTotalBytes = remoteBuild.UncompressedTotalBytes;

        InitializeDuplicatedChunkNames(context, remoteBuild.Manifests.Single().Data.Assets.SelectMany(a => a.AssetChunks));

        context.Progress.Report(new GamePackageOperationReport.Reset("Extracting", totalChunks, downloadTotalBytes, installTotalBytes));
        await context.Operation.Asset.InstallAssetsAsync(context, remoteBuild).ConfigureAwait(false);

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));
    }
}