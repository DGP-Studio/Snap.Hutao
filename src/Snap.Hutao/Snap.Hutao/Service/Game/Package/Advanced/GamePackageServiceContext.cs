// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
}