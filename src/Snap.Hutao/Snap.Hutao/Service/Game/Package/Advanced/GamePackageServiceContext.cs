// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.ContentDialog;
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
    public readonly TokenBucketRateLimiter? StreamCopyRateLimiter;

    private readonly IContentDialogFactory contentDialogFactory;
    private readonly AsyncKeyedLock<string> chunkLocks = new();

    public GamePackageServiceContext(GamePackageOperationContext operation, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, HttpClient httpClient, TokenBucketRateLimiter? rateLimiter, IContentDialogFactory contentDialogFactory)
    {
        Operation = operation;
        Progress = progress;
        ParallelOptions = parallelOptions;
        HttpClient = httpClient;
        StreamCopyRateLimiter = rateLimiter;
        this.contentDialogFactory = contentDialogFactory;
    }

    public CancellationToken CancellationToken { get => ParallelOptions.CancellationToken; }

    public async ValueTask<bool> EnsureAvailableFreeSpaceAsync(GamePackageOperationKind kind, long downloadTotalBytes, long totalBytes)
    {
        const long OneGigabyte = 1024L * 1024L * 1024L;
        long actualTotalBytes = totalBytes + OneGigabyte;
        long availableBytes = LogicalDriver.GetAvailableFreeSpace(Operation.ExtractOrGameDirectory);

        string downloadTotalBytesFormatted = Converters.ToFileSizeString(downloadTotalBytes);
        string totalBytesFormatted = Converters.ToFileSizeString(actualTotalBytes);
        string availableBytesFormatted = Converters.ToFileSizeString(availableBytes);

        string title = kind switch
        {
            GamePackageOperationKind.Install => SH.ServiceGamePackageAdvancedConfirmStartInstallTitle,
            GamePackageOperationKind.Update => SH.ServiceGamePackageAdvancedConfirmStartUpdateTitle,
            GamePackageOperationKind.Predownload => SH.ServiceGamePackageAdvancedConfirmStartPredownloadTitle,
            GamePackageOperationKind.ExtractBlk => "Start extracting game blocks?",
            GamePackageOperationKind.ExtractExe => "Start extracting game executable?",
            _ => throw HutaoException.NotSupported(),
        };
        string message = SH.FormatServiceGamePackageAdvancedConfirmMessage(downloadTotalBytesFormatted, totalBytesFormatted, availableBytesFormatted);

        bool isFreeSpaceAvailable = actualTotalBytes <= availableBytes;
        if (!isFreeSpaceAvailable)
        {
            title = SH.FormatServiceGamePackageAdvancedDriverNoAvailableFreeSpace(totalBytesFormatted, availableBytesFormatted);
        }

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(title, message, isPrimaryButtonEnabled: isFreeSpaceAvailable)
            .ConfigureAwait(false);

        return result is ContentDialogResult.Primary;
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