// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Snap.Hutao.Core.IO;
using System.Collections.Concurrent;
using System.Net.Http;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal readonly struct GamePackageServiceContext
{
    public readonly GamePackageOperationContext Operation;
    public readonly IProgress<GamePackageOperationReport> Progress;
    public readonly ParallelOptions ParallelOptions;
    public readonly ConcurrentDictionary<string, Void> DuplicatedChunkNames = [];
    public readonly HttpClient HttpClient;

    private readonly AsyncKeyedLock<string> chunkLocks = new();

    public GamePackageServiceContext(GamePackageOperationContext operation, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, HttpClient httpClient)
    {
        Operation = operation;
        Progress = progress;
        ParallelOptions = parallelOptions;
        HttpClient = httpClient;
    }

    public CancellationToken CancellationToken { get => ParallelOptions.CancellationToken; }

    public bool EnsureAvailableFreeSpace(long totalBytes)
    {
        long availableBytes = LogicalDriver.GetAvailableFreeSpace(Operation.GameFileSystem.GameDirectory);

        if (totalBytes > availableBytes)
        {
            string totalBytesFormatted = Converters.ToFileSizeString(totalBytes);
            string availableBytesFormatted = Converters.ToFileSizeString(availableBytes);
            string title = SH.FormatServiceGamePackageAdvancedDriverNoAvailableFreeSpace(totalBytesFormatted, availableBytesFormatted);
            Progress.Report(new GamePackageOperationReport.Reset(title));
            return false;
        }

        return true;
    }

    [SuppressMessage("", "SH003")]
    public Task<AsyncKeyedLock<string>.Releaser> ExclusiveProcessChunkAsync(string chunkName, CancellationToken token)
    {
        return chunkLocks.LockAsync(chunkName);
    }
}