// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package.Advanced.PackageOperation;

[Injection(InjectAs.Transient, typeof(IGamePackageOperation), Key = GamePackageOperationKind.ExtractExecutable)]
internal sealed class GamePackageExtractExecutableOperation : GamePackageOperation
{
    public override async ValueTask ExecuteAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild remoteBuild = context.Operation.RemoteBuild;
        int totalChunks = remoteBuild.TotalChunks;
        long totalBytes = remoteBuild.UncompressedTotalBytes;

        InitializeDuplicatedChunkNames(context, remoteBuild.Manifests.Single().Data.Assets.SelectMany(a => a.AssetChunks));

        context.Progress.Report(new GamePackageOperationReport.Reset("Extracting", totalChunks, totalBytes));
        await context.Operation.Asset.InstallAssetsAsync(context, remoteBuild).ConfigureAwait(false);

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));
    }
}