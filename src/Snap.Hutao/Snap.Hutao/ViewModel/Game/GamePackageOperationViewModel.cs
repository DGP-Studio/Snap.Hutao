// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Dispatching;
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

    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private readonly object syncRoot = new();

    private readonly IGamePackageService gamePackageService;

    private ValueStopwatch stopwatch = ValueStopwatch.StartNew();
    private long totalBytesReadPerSecond;
    private long totalBytesRead;
    private long contentLength;

    private int finishedBlocks;
    private int totalBlocks = -1;
    private bool forceFinished;

    private string title = "正在拉取清单数据";
    private string speed = "0 B/s";
    private string remainingTime = UnknownRemainingTime;

    public string Title { get => title; private set => SetProperty(ref title, value); }

    public string Speed { get => speed; private set => SetProperty(ref speed, value); }

    public string RemainingTime { get => remainingTime; private set => SetProperty(ref remainingTime, value); }

    public double Progress { get => totalBlocks <= 0 ? 0D : 1D * finishedBlocks / totalBlocks; }

    public string ProgressPercent { get => $"{Progress:P2}"; }

    public string BlockProgress { get => totalBlocks > -1 ? $"{finishedBlocks} / {totalBlocks}" : "正在加载清单文件"; }

    public bool IsIndeterminate { get => totalBlocks is -1; }

    public bool IsFinished { get => forceFinished || (totalBlocks is not -1 && finishedBlocks >= totalBlocks); }

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

    private void UpdateProgress(GamePackageOperationReport.Update update)
    {
        Interlocked.Add(ref totalBytesRead, update.BytesRead);
        Interlocked.Add(ref totalBytesReadPerSecond, update.BytesRead);
        Interlocked.Add(ref finishedBlocks, update.Blocks);

        if (stopwatch.GetElapsedTime().TotalMilliseconds > 1000)
        {
            lock (syncRoot)
            {
                if (stopwatch.GetElapsedTime().TotalMilliseconds > 1000)
                {
                    dispatcherQueue.TryEnqueue(() =>
                    {
                        Speed = $"{Converters.ToFileSizeString(totalBytesReadPerSecond),8}/s";
                        RemainingTime = totalBytesReadPerSecond == 0
                            ? UnknownRemainingTime
                            : TimeSpan.FromSeconds((double)(contentLength - totalBytesRead) / totalBytesReadPerSecond).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);

                        totalBytesReadPerSecond = 0;
                        RefreshUI();
                    });

                    stopwatch = ValueStopwatch.StartNew();
                }
            }
        }
    }

    private void ResetProgress(GamePackageOperationReport.Reset reset)
    {
        stopwatch = ValueStopwatch.StartNew();

        dispatcherQueue.TryEnqueue(() =>
        {
            finishedBlocks = 0;
            totalBytesRead = 0;
            totalBytesReadPerSecond = 0;
            totalBlocks = reset.TotalBlocks;
            contentLength = reset.ContentLength;
            Title = reset.Title;
            RefreshUI();
        });
    }

    private void FinishProgress(GamePackageOperationReport.Finish finish)
    {
        dispatcherQueue.TryEnqueue(() =>
        {
            Title = finish.OperationKind switch
            {
                GamePackageOperationKind.Install => "安装完成",
                GamePackageOperationKind.Verify => finish.Repaired ? "修复完成" : "游戏完整，无需修复",
                GamePackageOperationKind.Update => "更新完成",
                GamePackageOperationKind.Predownload => "预下载完成",
                _ => throw HutaoException.NotSupported(),
            };

            RefreshUI();
        });
    }

    private void RefreshUI()
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
        gamePackageService.CancelOperationAsync().SafeForget();
        forceFinished = true;
        dispatcherQueue.TryEnqueue(() =>
        {
            Title = "已取消";
            OnPropertyChanged(nameof(IsFinished));
        });
    }
}
