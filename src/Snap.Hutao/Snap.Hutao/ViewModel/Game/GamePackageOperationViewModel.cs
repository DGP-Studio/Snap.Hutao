// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Service.Game.Package;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Game;

internal sealed partial class GamePackageOperationViewModel : ObservableObject, IProgress<SophonChunkDownloadStatus>
{
    private readonly object syncRoot = new();
    private ValueStopwatch stopwatch = ValueStopwatch.StartNew();
    private long totalBytesReadPerSecond;
    private long totalBytesRead;
    private long contentLength;

    private string title;
    private int finishedBlocks;
    private int totalBlocks;
    private string speed;
    private bool forceFinished;
    private string remainingTime;

    public GamePackageOperationViewModel(string title)
    {
        this.title = title;

        totalBlocks = -1;
        speed = "0 B/s";
        remainingTime = "未知";
    }

    public string Title { get => title; set => SetProperty(ref title, value); }

    public int FinishedBlocks
    {
        get => finishedBlocks; set
        {
            SetProperty(ref finishedBlocks, value);
            RefreshUI();
        }
    }

    public double Progress { get => totalBlocks <= 0 ? 100.0 : Math.Round(100.0 * finishedBlocks / totalBlocks, 2); }

    public string ProgressPercent { get => $"{Progress:F2}%"; }

    public string BlockProgress { get => totalBlocks > -1 ? $"{finishedBlocks} / {totalBlocks}" : "正在加载清单文件"; }

    public string Speed { get => speed; set => SetProperty(ref speed, value); }

    public string RemainingTime { get => remainingTime; set => SetProperty(ref remainingTime, value); }

    public bool IsIndeterminate { get => totalBlocks is -1; }

    public bool IsFinished { get => forceFinished || (totalBlocks is not -1 && finishedBlocks >= totalBlocks); }

    public void ResetProgress(int totalBlocks, long contentLength)
    {
        finishedBlocks = 0;
        totalBytesRead = 0;
        totalBytesReadPerSecond = 0;
        this.totalBlocks = totalBlocks;
        this.contentLength = contentLength;
        stopwatch = ValueStopwatch.StartNew();

        RefreshUI();
    }

    public void Report(SophonChunkDownloadStatus status)
    {
        Interlocked.Add(ref totalBytesRead, status.BytesRead);
        Interlocked.Add(ref totalBytesReadPerSecond, status.BytesRead);
        if (status.Finished)
        {
            Interlocked.Add(ref finishedBlocks, 1);
        }

        if (stopwatch.GetElapsedTime().TotalMilliseconds > 1000)
        {
            lock (syncRoot)
            {
                if (stopwatch.GetElapsedTime().TotalMilliseconds > 1000)
                {
                    Speed = $"{Converters.ToFileSizeString(totalBytesReadPerSecond),8}/s";
                    RemainingTime = totalBytesReadPerSecond == 0
                        ? "未知"
                        : TimeSpan.FromSeconds((double)(contentLength - totalBytesRead) / totalBytesReadPerSecond).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);

                    totalBytesReadPerSecond = 0;
                    RefreshUI();

                    stopwatch = ValueStopwatch.StartNew();
                }
            }
        }
    }

    public void RefreshUI()
    {
        OnPropertyChanged(nameof(Progress));
        OnPropertyChanged(nameof(ProgressPercent));
        OnPropertyChanged(nameof(BlockProgress));
        OnPropertyChanged(nameof(IsIndeterminate));
        OnPropertyChanged(nameof(IsFinished));
    }

    [Command("CancelCommand")]
    private void Cancel()
    {
        IGamePackageService gamePackageService = Ioc.Default.GetRequiredService<IGamePackageService>();
        gamePackageService.CancelOperationAsync().SafeForget();
        Title = "已取消";
        forceFinished = true;
        OnPropertyChanged(nameof(IsFinished));
    }
}
