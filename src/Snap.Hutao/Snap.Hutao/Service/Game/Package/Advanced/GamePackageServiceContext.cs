// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Snap.Hutao.Core.ComponentModel;
using Snap.Hutao.Core.IO;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal readonly struct GamePackageServiceContext
{
    public readonly GamePackageOperationContext Operation;
    public readonly IProgress<GamePackageOperationReport> Progress;
    public readonly ParallelOptions ParallelOptions;
    public readonly ConcurrentDictionary<string, Void> DuplicatedChunkNames = [];
    public readonly HttpClient HttpClient;
    public readonly IAsyncDisposableObservableBox<TokenBucketRateLimiter?> StreamCopyRateLimiter;

    private readonly AsyncKeyedLock<string> chunkLocks = new();

    public GamePackageServiceContext(GamePackageOperationContext operation, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, HttpClient httpClient, IAsyncDisposableObservableBox<TokenBucketRateLimiter?> rateLimiter)
    {
        Operation = operation;
        Progress = progress;
        ParallelOptions = parallelOptions;
        HttpClient = httpClient;
        StreamCopyRateLimiter = rateLimiter;
    }

    public CancellationToken CancellationToken { get => ParallelOptions.CancellationToken; }

    public bool EnsureAvailableFreeSpace(long totalBytes)
    {
        const long OneGigabyte = 1024L * 1024L * 1024L;
        long actualTotalBytes = totalBytes + OneGigabyte;
        long availableBytes = LogicalDriver.GetAvailableFreeSpace(Operation.ExtractOrGameDirectory);

        if (actualTotalBytes > availableBytes)
        {
            string totalBytesFormatted = Converters.ToFileSizeString(actualTotalBytes);
            string availableBytesFormatted = Converters.ToFileSizeString(availableBytes);
            string title = SH.FormatServiceGamePackageAdvancedDriverNoAvailableFreeSpace(totalBytesFormatted, availableBytesFormatted);
            Progress.Report(new GamePackageOperationReport.Reset(title));
            return false;
        }

        return true;
    }

    public string EnsureAssetTargetDirectoryExists(string assetName)
    {
        if (Operation.Kind is GamePackageOperationKind.ExtractBlk or GamePackageOperationKind.ExtractExe)
        {
            assetName = Path.GetFileName(assetName);
        }

        string targetPath = Path.Combine(Operation.ExtractOrGameDirectory, assetName);

        string? directory = Path.GetDirectoryName(targetPath);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);

        return targetPath;
    }

    [SuppressMessage("", "SH003")]
    public Task<AsyncKeyedLock<string>.Releaser> ExclusiveProcessChunkAsync(string chunkName, CancellationToken token)
    {
        return chunkLocks.LockAsync(chunkName);
    }
}