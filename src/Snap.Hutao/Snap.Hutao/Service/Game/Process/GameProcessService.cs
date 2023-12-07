// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Discord;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Game.Unlocker;
using System.IO;
using static Snap.Hutao.Service.Game.GameConstants;

namespace Snap.Hutao.Service.Game.Process;

/// <summary>
/// 进程互操作
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameProcessService))]
internal sealed partial class GameProcessService : IGameProcessService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IDiscordService discordService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly LaunchOptions launchOptions;
    private readonly AppOptions appOptions;

    private volatile bool isGameRunning;

    public bool IsGameRunning()
    {
        if (isGameRunning)
        {
            return true;
        }

        // Original two GetProcessesByName is O(2n)
        // GetProcesses once and manually loop is O(n)
        foreach (ref System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses().AsSpan())
        {
            if (process.ProcessName is YuanShenProcessName or GenshinImpactProcessName)
            {
                return true;
            }
        }

        return false;
    }

    public async ValueTask LaunchAsync(IProgress<LaunchStatus> progress)
    {
        if (IsGameRunning())
        {
            return;
        }

        if (!appOptions.TryGetGamePathAndGameFileName(out string gamePath, out string? gameFileName))
        {
            ArgumentException.ThrowIfNullOrEmpty(gamePath);
            return; // null check passing, actually never reach.
        }

        bool isOversea = LaunchScheme.ExecutableIsOversea(gameFileName);

        progress.Report(new(LaunchPhase.ProcessInitializing, SH.ServiceGameLaunchPhaseProcessInitializing));
        using (System.Diagnostics.Process game = InitializeGameProcess(gamePath))
        {
            await using (await GameRunningTracker.CreateAsync(this, isOversea).ConfigureAwait(false))
            {
                game.Start();
                progress.Report(new(LaunchPhase.ProcessStarted, SH.ServiceGameLaunchPhaseProcessStarted));

                if (launchOptions.UseStarwardPlayTimeStatistics)
                {
                    await Starward.LaunchForPlayTimeStatisticsAsync(isOversea).ConfigureAwait(false);
                }

                if (runtimeOptions.IsElevated && appOptions.IsAdvancedLaunchOptionsEnabled && launchOptions.UnlockFps)
                {
                    progress.Report(new(LaunchPhase.UnlockingFps, SH.ServiceGameLaunchPhaseUnlockingFps));
                    try
                    {
                        await UnlockFpsAsync(game, progress).ConfigureAwait(false);
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
        }
    }

    private System.Diagnostics.Process InitializeGameProcess(string gamePath)
    {
        string commandLine = string.Empty;

        if (launchOptions.IsEnabled)
        {
            // https://docs.unity.cn/cn/current/Manual/PlayerCommandLineArguments.html
            // https://docs.unity3d.com/2017.4/Documentation/Manual/CommandLineArguments.html
            commandLine = new CommandLineBuilder()
                .AppendIf("-popupwindow", launchOptions.IsBorderless)
                .AppendIf("-window-mode", launchOptions.IsExclusive, "exclusive")
                .Append("-screen-fullscreen", launchOptions.IsFullScreen ? 1 : 0)
                .AppendIf("-screen-width", launchOptions.IsScreenWidthEnabled, launchOptions.ScreenWidth)
                .AppendIf("-screen-height", launchOptions.IsScreenHeightEnabled, launchOptions.ScreenHeight)
                .AppendIf("-monitor", launchOptions.IsMonitorEnabled, launchOptions.Monitor.Value)
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

    private ValueTask UnlockFpsAsync(System.Diagnostics.Process game, IProgress<LaunchStatus> progress, CancellationToken token = default)
    {
#pragma warning disable CA1859
        IGameFpsUnlocker unlocker = serviceProvider.CreateInstance<GameFpsUnlocker>(game);
#pragma warning restore CA1859
        UnlockTimingOptions options = new(100, 20000, 3000);
        Progress<UnlockerStatus> lockerProgress = new(unlockStatus => progress.Report(LaunchStatus.FromUnlockStatus(unlockStatus)));
        return unlocker.UnlockAsync(options, lockerProgress, token);
    }

    private class GameRunningTracker : IAsyncDisposable
    {
        private readonly GameProcessService service;
        private readonly bool previousSetDiscordActivityWhenPlaying;

        private GameRunningTracker(GameProcessService service, bool isOversea)
        {
            service.isGameRunning = true;
            previousSetDiscordActivityWhenPlaying = service.launchOptions.SetDiscordActivityWhenPlaying;
            this.service = service;
        }

        public static async ValueTask<GameRunningTracker> CreateAsync(GameProcessService service, bool isOversea)
        {
            GameRunningTracker tracker = new(service, isOversea);
            if (tracker.previousSetDiscordActivityWhenPlaying)
            {
                await service.discordService.SetPlayingActivity(isOversea).ConfigureAwait(false);
            }

            return tracker;
        }

        public async ValueTask DisposeAsync()
        {
            if (previousSetDiscordActivityWhenPlaying)
            {
                await service.discordService.SetNormalActivity().ConfigureAwait(false);
            }

            service.isGameRunning = false;
        }
    }
}