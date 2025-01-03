// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Game.Package.Advanced;
using System.Collections.Frozen;
using System.Diagnostics;

namespace Snap.Hutao.ViewModel.Game;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class GamePackageOperationViewModel : Abstraction.ViewModel
{
    private const string ZeroBytesPerSecondSpeed = "0 bytes/s";
    private const string UnknownRemainingTime = "99:59:59";

    private static readonly TimeSpan ProgressTimeout = TimeSpan.FromSeconds(5);

    private readonly FrozenDictionary<GamePackageOperationReportKind, Lock> syncRoots = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(GamePackageOperationReportKind.Download, new Lock()),
        KeyValuePair.Create(GamePackageOperationReportKind.Install, new Lock()),
    ]);

    private readonly ILogger<GamePackageOperationViewModel> logger;
    private readonly IGamePackageService gamePackageService;
    private readonly ITaskContext taskContext;

    private long bytesDownloadedSinceLastUpdate;
    private long totalBytesDownloaded;
    private long bytesDownloadedLastRefreshTime;
    private long bytesInstalledSinceLastUpdate;
    private long totalBytesInstalled;
    private long bytesInstalledLastRefreshTime;
    private long contentLength;

    public partial AppOptions AppOptions { get; }

    public string Title { get; private set => SetProperty(ref field, value); } = SH.UIXamlViewSpecializedSophonProgressDefault;

    public int DownloadedChunks { get; private set; }

    public string DownloadFileName { get; private set; } = default!;

    public string DownloadSpeed { get; private set => SetProperty(ref field, value); } = ZeroBytesPerSecondSpeed;

    public string DownloadRemainingTime { get; private set => SetProperty(ref field, value); } = UnknownRemainingTime;

    public int InstalledChunks { get; private set; }

    public string InstallFileName { get; private set; } = default!;

    public string InstallSpeed { get; private set => SetProperty(ref field, value); } = ZeroBytesPerSecondSpeed;

    public string InstallRemainingTime { get; private set => SetProperty(ref field, value); } = UnknownRemainingTime;

    public int DownloadTotalChunks { get; private set => SetProperty(ref field, value); } = -1;

    public int InstallTotalChunks { get; private set => SetProperty(ref field, value); } = -1;

    public bool IsFinished { get; private set => SetProperty(ref field, value); }

    public void HandleProgressUpdate(GamePackageOperationReport status)
    {
        switch (status)
        {
            case GamePackageOperationReport.Reset reset:
                ResetProgress(reset);
                return;
            case GamePackageOperationReport.Update update:
                UpdateProgress(update);
                return;
            case GamePackageOperationReport.Finish finish:
                FinishProgress(finish);
                break;
        }
    }

    internal void TestProgress()
    {
        Title = "HOMO";
        DownloadedChunks = 114514;
        DownloadSpeed = "11.45 MB/s";
        DownloadFileName = "Hill";
        DownloadRemainingTime = "11:45:14";
        DownloadTotalChunks = 1919810;
        InstalledChunks = 114514;
        InstallFileName = "HomoHat";
        InstallSpeed = "19.19 MB/s";
        InstallRemainingTime = "19:19:810";
        InstallTotalChunks = 191981;
        RefreshUI();
    }

    private void UpdateProgress(GamePackageOperationReport.Update update)
    {
        long current = Stopwatch.GetTimestamp();
        lock (syncRoots[update.Kind])
        {
            switch (update)
            {
                case GamePackageOperationReport.Download download:
                    {
                        UpdateDownloadProgress(download);
                        TimeSpan elapsedTime = Stopwatch.GetElapsedTime(bytesDownloadedLastRefreshTime);
                        if (elapsedTime.TotalMilliseconds < 1000)
                        {
                            return;
                        }

                        bytesDownloadedLastRefreshTime = current;
                        long bytesDownloadedPerSecond = (long)(bytesDownloadedSinceLastUpdate / elapsedTime.TotalSeconds);
                        DownloadSpeed = $"{Converters.ToFileSizeString(bytesDownloadedPerSecond),8}/s";
                        logger.LogInformation("Download Info: [{Bytes}KB|{Duration}|{Speed}]", bytesDownloadedSinceLastUpdate / 1024D, elapsedTime, DownloadSpeed);
                        DownloadRemainingTime = bytesDownloadedPerSecond is 0
                            ? UnknownRemainingTime
                            : $"{TimeSpan.FromSeconds((double)(contentLength - totalBytesDownloaded) / bytesDownloadedPerSecond):hh\\:mm\\:ss}";

                        bytesDownloadedSinceLastUpdate = 0;
                    }

                    break;
                case GamePackageOperationReport.Install install:
                    {
                        UpdateInstallProgress(install);
                        TimeSpan elapsedTime = Stopwatch.GetElapsedTime(bytesInstalledLastRefreshTime);
                        if (elapsedTime.TotalMilliseconds < 1000)
                        {
                            return;
                        }

                        bytesInstalledLastRefreshTime = current;
                        long bytesInstalledPerSecond = (long)(bytesInstalledSinceLastUpdate / elapsedTime.TotalSeconds);
                        InstallSpeed = $"{Converters.ToFileSizeString(bytesInstalledPerSecond),8}/s";
                        InstallRemainingTime = bytesInstalledPerSecond is 0
                            ? UnknownRemainingTime
                            : $"{TimeSpan.FromSeconds((double)(contentLength - totalBytesInstalled) / bytesInstalledPerSecond):hh\\:mm\\:ss}";

                        bytesInstalledSinceLastUpdate = 0;
                    }

                    break;
                default:
                    HutaoException.NotSupported();
                    break;
            }

            RefreshUI();
        }
    }

    private void UpdateDownloadProgress(GamePackageOperationReport.Download download)
    {
        totalBytesDownloaded += download.BytesRead;
        bytesDownloadedSinceLastUpdate += download.BytesRead;
        DownloadedChunks += download.Chunks;
        DownloadFileName = download.FileName;
    }

    private void UpdateInstallProgress(GamePackageOperationReport.Install install)
    {
        totalBytesInstalled += install.BytesRead;
        bytesInstalledSinceLastUpdate += install.BytesRead;
        InstalledChunks += install.Chunks;
        InstallFileName = install.FileName;
    }

    private void ResetProgress(GamePackageOperationReport.Reset reset)
    {
        DownloadedChunks = 0;
        InstalledChunks = 0;
        totalBytesDownloaded = 0;
        bytesDownloadedSinceLastUpdate = 0;
        bytesDownloadedLastRefreshTime = Stopwatch.GetTimestamp();
        totalBytesInstalled = 0;
        bytesInstalledSinceLastUpdate = 0;
        bytesInstalledLastRefreshTime = Stopwatch.GetTimestamp();
        contentLength = reset.ContentLength;
        DownloadTotalChunks = reset.DownloadTotalChunks;
        DownloadFileName = default!;
        InstallTotalChunks = reset.InstallTotalChunks;
        InstallFileName = default!;
        Title = reset.Title;
        RefreshUI();
    }

    private void FinishProgress(GamePackageOperationReport.Finish finish)
    {
        Title = finish.OperationKind switch
        {
            GamePackageOperationKind.Install => SH.ViewModelGamePakcageOperationCompleteInstall,
            GamePackageOperationKind.Verify => finish.Repaired ? SH.ViewModelGamePakcageOperationCompleteRepair : SH.ViewModelGamePakcageOperationSkipRepair,
            GamePackageOperationKind.Update => SH.ViewModelGamePakcageOperationCompleteUpdate,
            GamePackageOperationKind.ExtractBlk or GamePackageOperationKind.ExtractExe => "Extracted",
            GamePackageOperationKind.Predownload => SH.ViewModelGamePakcageOperationCompletePredownload,
            _ => throw HutaoException.NotSupported(),
        };

        IsFinished = true;
        RefreshUI();
    }

    private void RefreshUI()
    {
        OnPropertyChanged(nameof(DownloadedChunks));
        OnPropertyChanged(nameof(InstalledChunks));
        OnPropertyChanged(nameof(DownloadFileName));
        OnPropertyChanged(nameof(InstallFileName));
    }

    [Command("PeriodicRefreshUICommand")]
    private async Task PeriodicRefreshUIAsync()
    {
        using (PeriodicTimer timer = new(TimeSpan.FromSeconds(5)))
        {
            do
            {
                RefreshCore();
                await timer.WaitForNextTickAsync(CancellationToken).ConfigureAwait(false);
            }
            while (!IsFinished && !CancellationToken.IsCancellationRequested);
        }

        void RefreshCore()
        {
            TimeSpan elapsedTimeSinceLastDownloadReport = Stopwatch.GetElapsedTime(bytesDownloadedLastRefreshTime);
            TimeSpan elapsedTimeSinceLastInstallReport = Stopwatch.GetElapsedTime(bytesInstalledLastRefreshTime);

            if (elapsedTimeSinceLastDownloadReport > ProgressTimeout)
            {
                taskContext.InvokeOnMainThread(() =>
                {
                    DownloadSpeed = ZeroBytesPerSecondSpeed;
                    DownloadRemainingTime = UnknownRemainingTime;
                });
            }

            if (elapsedTimeSinceLastInstallReport > ProgressTimeout)
            {
                taskContext.InvokeOnMainThread(() =>
                {
                    InstallSpeed = ZeroBytesPerSecondSpeed;
                    InstallRemainingTime = UnknownRemainingTime;
                });
            }
        }
    }

    [Command("CancelCommand")]
    private async Task CancelAsync()
    {
        Title = SH.ViewModelGamePakcageOperationCancelling;
        await gamePackageService.CancelOperationAsync().ConfigureAwait(true);
        IsFinished = true;
        Title = SH.ViewModelGamePakcageOperationCanceled;
    }
}