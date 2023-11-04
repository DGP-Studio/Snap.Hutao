// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Game.Unlocker;
using System.Diagnostics;
using System.IO;
using static Snap.Hutao.Service.Game.GameConstants;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 进程互操作
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameProcessService))]
internal sealed partial class GameProcessService : IGameProcessService
{
    private readonly IServiceProvider serviceProvider;
    private readonly RuntimeOptions runtimeOptions;
    private readonly LaunchOptions launchOptions;
    private readonly AppOptions appOptions;

    private volatile int runningGamesCounter;

    public bool IsGameRunning()
    {
        if (runningGamesCounter == 0)
        {
            return false;
        }

        return Process.GetProcessesByName(YuanShenProcessName).Any()
            || Process.GetProcessesByName(GenshinImpactProcessName).Any();
    }

    public async ValueTask LaunchAsync(IProgress<LaunchStatus> progress)
    {
        if (IsGameRunning())
        {
            return;
        }

        string gamePath = appOptions.GamePath;
        ArgumentException.ThrowIfNullOrEmpty(gamePath);

        progress.Report(new(LaunchPhase.ProcessInitializing, SH.ServiceGameLaunchPhaseProcessInitializing));
        using (Process game = InitializeGameProcess(launchOptions, gamePath))
        {
            try
            {
                Interlocked.Increment(ref runningGamesCounter);
                game.Start();
                progress.Report(new(LaunchPhase.ProcessStarted, SH.ServiceGameLaunchPhaseProcessStarted));

                if (runtimeOptions.IsElevated && appOptions.IsAdvancedLaunchOptionsEnabled && launchOptions.UnlockFps)
                {
                    progress.Report(new(LaunchPhase.UnlockingFps, SH.ServiceGameLaunchPhaseUnlockingFps));
                    try
                    {
                        await UnlockFpsAsync(serviceProvider, game, progress).ConfigureAwait(false);
                    }
                    catch (InvalidOperationException)
                    {
                        // The Unlocker can't unlock the process
                        game.Kill();
                        throw;
                    }
                    finally
                    {
                        progress.Report(new(LaunchPhase.ProcessExited, SH.ServiceGameLaunchPhaseProcessExited));
                    }
                }
                else
                {
                    progress.Report(new(LaunchPhase.WaitingForExit, SH.ServiceGameLaunchPhaseWaitingProcessExit));
                    await game.WaitForExitAsync().ConfigureAwait(false);
                    progress.Report(new(LaunchPhase.ProcessExited, SH.ServiceGameLaunchPhaseProcessExited));
                }
            }
            finally
            {
                Interlocked.Decrement(ref runningGamesCounter);
            }
        }
    }

    private static Process InitializeGameProcess(LaunchOptions options, string gamePath)
    {
        string commandLine = string.Empty;

        if (options.IsEnabled)
        {
            Must.Argument(!(options.IsBorderless && options.IsExclusive), "无边框与独占全屏选项无法同时生效");

            // https://docs.unity.cn/cn/current/Manual/PlayerCommandLineArguments.html
            // https://docs.unity3d.com/2017.4/Documentation/Manual/CommandLineArguments.html
            commandLine = new CommandLineBuilder()
                .AppendIf("-popupwindow", options.IsBorderless)
                .AppendIf("-window-mode", options.IsExclusive, "exclusive")
                .Append("-screen-fullscreen", options.IsFullScreen ? 1 : 0)
                .AppendIf("-screen-width", options.IsScreenWidthEnabled, options.ScreenWidth)
                .AppendIf("-screen-height", options.IsScreenHeightEnabled, options.ScreenHeight)
                .AppendIf("-monitor", options.IsMonitorEnabled, options.Monitor.Value)
                .ToString();
        }

        return new()
        {
            StartInfo = new()
            {
                Arguments = commandLine,
                FileName = gamePath,
                UseShellExecute = true,
                Verb = "runas",
                WorkingDirectory = Path.GetDirectoryName(gamePath),
            },
        };
    }

    private static ValueTask UnlockFpsAsync(IServiceProvider serviceProvider, Process game, IProgress<LaunchStatus> progress, CancellationToken token = default)
    {
        IGameFpsUnlocker unlocker = serviceProvider.CreateInstance<GameFpsUnlocker>(game);
        UnlockTimingOptions options = new(100, 20000, 3000);
        Progress<UnlockerStatus> lockerProgress = new(unlockStatus => progress.Report(LaunchStatus.FromUnlockStatus(unlockStatus)));
        return unlocker.UnlockAsync(options, lockerProgress, token);
    }
}