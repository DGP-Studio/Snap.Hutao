// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.Package.Advanced;

namespace Snap.Hutao.ViewModel.Game;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class GamePackageOperationViewModel : Abstraction.ViewModel
{
    private const string ZeroBytesPerSecondSpeed = "0 bytes/s";
    private const string UnknownRemainingTime = "--:--:--";

    private readonly ILogger<GamePackageOperationViewModel> logger;
    private readonly IGamePackageService gamePackageService;
    private readonly ITaskContext taskContext;

    private long bytesDownloadedSinceLastUpdate;
    private long totalBytesDownloaded;
    private long bytesInstalledSinceLastUpdate;
    private long totalBytesInstalled;
    private long contentLength;
    private int downloadedChunks;
    private int installedChunks;
    private int downloadTotalChunks = -1;
    private int installTotalChunks = -1;
    private bool isFinished;

    private string title = SH.UIXamlViewSpecializedSophonProgressDefault;
    private string downloadSpeed = ZeroBytesPerSecondSpeed;
    private string downloadRemainingTime = UnknownRemainingTime;
    private string installSpeed = ZeroBytesPerSecondSpeed;
    private string installRemainingTime = UnknownRemainingTime;

    public string Title { get => title; private set => SetProperty(ref title, value); }

    public int DownloadedChunks { get => downloadedChunks; }

    public string DownloadFileName { get; private set; } = default!;

    public string DownloadSpeed { get => downloadSpeed; private set => SetProperty(ref downloadSpeed, value); }

    public string DownloadRemainingTime { get => downloadRemainingTime; private set => SetProperty(ref downloadRemainingTime, value); }

    public int InstalledChunks { get => installedChunks; }

    public string InstallFileName { get; private set; } = default!;

    public string InstallSpeed { get => installSpeed; private set => SetProperty(ref installSpeed, value); }

    public string InstallRemainingTime { get => installRemainingTime; private set => SetProperty(ref installRemainingTime, value); }

    public int DownloadTotalChunks { get => downloadTotalChunks; private set => SetProperty(ref downloadTotalChunks, value); }

    public int InstallTotalChunks { get => installTotalChunks; private set => SetProperty(ref installTotalChunks, value); }

    public bool IsFinished { get => isFinished; private set => SetProperty(ref isFinished, value); }

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
        downloadedChunks = 114514;
        DownloadSpeed = "11.45 MB/s";
        DownloadFileName = "Hill";
        DownloadRemainingTime = "11:45:14";
        DownloadTotalChunks = 1919810;
        installedChunks = 114514;
        InstallFileName = "HomoHat";
        InstallSpeed = "19.19 MB/s";
        InstallRemainingTime = "19:19:810";
        InstallTotalChunks = 191981;
        RefreshUI();
    }

    protected override ValueTask<bool> InitializeOverrideAsync()
    {
        PeriodicRefreshUIAsync().SafeForget(logger);
        return ValueTask.FromResult(true);
    }

    private void UpdateProgress(GamePackageOperationReport.Update update)
    {
        _ = update switch
        {
            GamePackageOperationReport.Download download => UpdateDownloadProgress(download),
            GamePackageOperationReport.Install install => UpdateInstallProgress(install),
            _ => throw HutaoException.NotSupported(),
        };
    }

    private Core.Void UpdateDownloadProgress(GamePackageOperationReport.Download download)
    {
        Interlocked.Add(ref totalBytesDownloaded, download.BytesRead);
        Interlocked.Add(ref bytesDownloadedSinceLastUpdate, download.BytesRead);
        Interlocked.Add(ref downloadedChunks, download.Chunks);
        DownloadFileName = download.FileName;
        return default;
    }

    private Core.Void UpdateInstallProgress(GamePackageOperationReport.Install install)
    {
        Interlocked.Add(ref totalBytesInstalled, install.BytesRead);
        Interlocked.Add(ref bytesInstalledSinceLastUpdate, install.BytesRead);
        Interlocked.Add(ref installedChunks, install.Chunks);
        InstallFileName = install.FileName;
        return default;
    }

    private void ResetProgress(GamePackageOperationReport.Reset reset)
    {
        downloadedChunks = 0;
        installedChunks = 0;
        totalBytesDownloaded = 0;
        bytesDownloadedSinceLastUpdate = 0;
        totalBytesInstalled = 0;
        bytesInstalledSinceLastUpdate = 0;
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

    private async ValueTask PeriodicRefreshUIAsync()
    {
        using (PeriodicTimer timer = new(TimeSpan.FromSeconds(1)))
        {
            do
            {
                taskContext.InvokeOnMainThread(RefreshCore);
                await timer.WaitForNextTickAsync().ConfigureAwait(true);
            }
            while (!IsFinished);
        }

        void RefreshCore()
        {
            long bytesDownloadedPerSecond = bytesDownloadedSinceLastUpdate;
            long bytesInstalledPerSecond = bytesInstalledSinceLastUpdate;

            DownloadSpeed = $"{Converters.ToFileSizeString(bytesDownloadedPerSecond),8}/s";
            DownloadRemainingTime = bytesDownloadedPerSecond is 0
                ? UnknownRemainingTime
                : $"{TimeSpan.FromSeconds((double)(contentLength - totalBytesDownloaded) / bytesDownloadedPerSecond):hh\\:mm\\:ss}";

            bytesDownloadedSinceLastUpdate = 0;

            InstallSpeed = $"{Converters.ToFileSizeString(bytesInstalledPerSecond),8}/s";
            InstallRemainingTime = bytesInstalledPerSecond is 0
                ? UnknownRemainingTime
                : $"{TimeSpan.FromSeconds((double)(contentLength - totalBytesInstalled) / bytesInstalledPerSecond):hh\\:mm\\:ss}";

            bytesInstalledSinceLastUpdate = 0;

            RefreshUI();
        }
    }

    [Command("CancelCommand")]
    private async Task Cancel()
    {
        Title = SH.ViewModelGamePakcageOperationCancelling;
        await gamePackageService.CancelOperationAsync().ConfigureAwait(true);
        IsFinished = true;
        Title = SH.ViewModelGamePakcageOperationCanceled;
    }
}