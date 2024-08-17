// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.Package.Advanced;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Game;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class GamePackageOperationViewModel : Abstraction.ViewModel
{
    private const string UnknownRemainingTime = "--:--:--";

    private readonly object syncRoot = new();

    private readonly IGamePackageService gamePackageService;

    private ValueStopwatch stopwatch = ValueStopwatch.StartNew();

    private long totalBytesDownloadedPerSecond;
    private long totalBytesDownloaded;

    private long totalBytesInstalledPerSecond;
    private long totalBytesInstalled;

    private long contentLength;

    private int downloadedChunks;
    private int installedChunks;
    private int downloadTotalChunks = -1;
    private int installTotalChunks = -1;
    private bool isFinished;

    private string title = "正在拉取清单数据";
    private string downloadSpeed = "0 B/s";
    private string downloadRemainingTime = UnknownRemainingTime;
    private string installSpeed = "0 B/s";
    private string installRemainingTime = UnknownRemainingTime;

    public string Title { get => title; private set => SetProperty(ref title, value); }

    public int DownloadedChunks { get => downloadedChunks; }

    public string DownloadCurrentChunkName { get; private set; } = default!;

    public string DownloadSpeed { get => downloadSpeed; private set => SetProperty(ref downloadSpeed, value); }

    public string DownloadRemainingTime { get => downloadRemainingTime; private set => SetProperty(ref downloadRemainingTime, value); }

    public int InstalledChunks { get => installedChunks; }

    public string InstallCurrentChunkName { get; private set; } = default!;

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

    public void TestProgress()
    {
        Title = "HOMO";
        downloadedChunks = 114514;
        DownloadSpeed = "11.45 MB/s";
        DownloadCurrentChunkName = "Hill";
        DownloadRemainingTime = "11:45:14";
        DownloadTotalChunks = 1919810;
        installedChunks = 114514;
        InstallCurrentChunkName = "HomoHat";
        InstallSpeed = "19.19 MB/s";
        InstallRemainingTime = "19:19:810";
        InstallTotalChunks = 191981;
        RefreshUI();
    }

    private void UpdateProgress(GamePackageOperationReport.Update update)
    {
        _ = update switch
        {
            GamePackageOperationReport.Download => UpdateDownloadProgress((GamePackageOperationReport.Download)update),
            GamePackageOperationReport.Install => UpdateInstallProgress((GamePackageOperationReport.Install)update),
            _ => throw HutaoException.NotSupported(),
        };

        if (stopwatch.GetElapsedTime().TotalMilliseconds > 1000)
        {
            lock (syncRoot)
            {
                if (stopwatch.GetElapsedTime().TotalMilliseconds > 1000)
                {
                    DownloadSpeed = $"{Converters.ToFileSizeString(totalBytesDownloadedPerSecond),8}/s";
                    DownloadRemainingTime = totalBytesDownloadedPerSecond == 0
                        ? UnknownRemainingTime
                        : TimeSpan.FromSeconds((double)(contentLength - totalBytesDownloaded) / totalBytesDownloadedPerSecond).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);

                    totalBytesDownloadedPerSecond = 0;

                    InstallSpeed = $"{Converters.ToFileSizeString(totalBytesInstalledPerSecond),8}/s";
                    InstallRemainingTime = totalBytesInstalledPerSecond == 0
                        ? UnknownRemainingTime
                        : TimeSpan.FromSeconds((double)(contentLength - totalBytesInstalled) / totalBytesInstalledPerSecond).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);

                    totalBytesInstalledPerSecond = 0;
                    RefreshUI();

                    stopwatch = ValueStopwatch.StartNew();
                }
            }
        }
    }

    private Core.Void UpdateDownloadProgress(GamePackageOperationReport.Download download)
    {
        Interlocked.Add(ref totalBytesDownloaded, download.BytesRead);
        Interlocked.Add(ref totalBytesDownloadedPerSecond, download.BytesRead);
        Interlocked.Add(ref downloadedChunks, download.Chunks);
        DownloadCurrentChunkName = download.CurrentChunkName;
        return default;
    }

    private Core.Void UpdateInstallProgress(GamePackageOperationReport.Install install)
    {
        Interlocked.Add(ref totalBytesInstalled, install.BytesRead);
        Interlocked.Add(ref totalBytesInstalledPerSecond, install.BytesRead);
        Interlocked.Add(ref installedChunks, install.Chunks);
        InstallCurrentChunkName = install.CurrentChunkName;
        return default;
    }

    private void ResetProgress(GamePackageOperationReport.Reset reset)
    {
        stopwatch = ValueStopwatch.StartNew();

        downloadedChunks = 0;
        installedChunks = 0;
        totalBytesDownloaded = 0;
        totalBytesDownloadedPerSecond = 0;
        totalBytesInstalled = 0;
        totalBytesInstalledPerSecond = 0;
        contentLength = reset.ContentLength;
        DownloadTotalChunks = reset.DownloadTotalChunks;
        DownloadCurrentChunkName = default!;
        InstallTotalChunks = reset.InstallTotalChunks;
        InstallCurrentChunkName = default!;
        Title = reset.Title;
        RefreshUI();
    }

    private void FinishProgress(GamePackageOperationReport.Finish finish)
    {
        Title = finish.OperationKind switch
        {
            GamePackageOperationKind.Install => "安装完成",
            GamePackageOperationKind.Verify => finish.Repaired ? "修复完成" : "游戏完整，无需修复",
            GamePackageOperationKind.Update => "更新完成",
            GamePackageOperationKind.Predownload => "预下载完成",
            _ => throw HutaoException.NotSupported(),
        };

        IsFinished = true;
    }

    private void RefreshUI()
    {
        OnPropertyChanged(nameof(DownloadedChunks));
        OnPropertyChanged(nameof(InstalledChunks));
        OnPropertyChanged(nameof(DownloadCurrentChunkName));
        OnPropertyChanged(nameof(InstallCurrentChunkName));
    }

    [Command("CancelCommand")]
    private void Cancel()
    {
        gamePackageService.CancelOperationAsync().SafeForget();
        IsFinished = true;
        Title = "已取消";
    }
}
