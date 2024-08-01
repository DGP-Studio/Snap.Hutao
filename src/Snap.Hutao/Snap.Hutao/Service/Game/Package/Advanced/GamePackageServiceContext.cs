// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Snap.Hutao.Core.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal readonly struct GamePackageServiceContext
{
    public readonly GamePackageOperationContext Operation;
    public readonly IProgress<GamePackageOperationReport> Progress;
    public readonly ParallelOptions ParallelOptions;

    public GamePackageServiceContext(GamePackageOperationContext operation, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        Operation = operation;
        Progress = progress;
        ParallelOptions = parallelOptions;
    }

    public CancellationToken CancellationToken { get => ParallelOptions.CancellationToken; }

    public bool EnsureAvailableFreeSpace(long totalBytes)
    {
        long availableBytes = LogicalDriver.GetAvailableFreeSpace(Operation.GameFileSystem.GameDirectory);

        if (totalBytes > availableBytes)
        {
            string title = $"磁盘空间不足，需要 {Converters.ToFileSizeString(totalBytes)}，剩余 {Converters.ToFileSizeString(availableBytes)}";
            Progress.Report(new GamePackageOperationReport.Reset(title));
            return false;
        }

        return true;
    }
}