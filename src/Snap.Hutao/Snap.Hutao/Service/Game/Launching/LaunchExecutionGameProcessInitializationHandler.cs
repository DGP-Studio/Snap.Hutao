// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Discord;
using System.IO;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class LaunchExecutionGameProcessInitializationHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (!context.Options.TryGetGamePathAndGameFileName(out string gamePath, out string? gameFileName))
        {
            context.Result.Kind = LaunchExecutionResultKind.NoActiveGamePath;
            context.Result.ErrorMessage = SH.ServiceGameLaunchExecutionGamePathNotValid;
            return;
        }

        context.Progress.Report(new(LaunchPhase.ProcessInitializing, SH.ServiceGameLaunchPhaseProcessInitializing));
        using (System.Diagnostics.Process game = InitializeGameProcess(context, gamePath))
        {
            await next().ConfigureAwait(false);

            // TODO: move to new handlers
            await using (await GameRunningTracker.CreateAsync(this, isOversea).ConfigureAwait(false))
            {
                game.Start();
                progress.Report(new(LaunchPhase.ProcessStarted, SH.ServiceGameLaunchPhaseProcessStarted));

                if (launchOptions.UseStarwardPlayTimeStatistics)
                {
                    await Starward.LaunchForPlayTimeStatisticsAsync(isOversea).ConfigureAwait(false);
                }

                if (runtimeOptions.IsElevated && launchOptions.IsAdvancedLaunchOptionsEnabled && launchOptions.UnlockFps)
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

    private static System.Diagnostics.Process InitializeGameProcess(LaunchExecutionContext context, string gamePath)
    {
        LaunchOptions launchOptions = context.Options;

        string commandLine = string.Empty;
        if (launchOptions.IsEnabled)
        {
            // https://docs.unity.cn/cn/current/Manual/PlayerCommandLineArguments.html
            // https://docs.unity3d.com/2017.4/Documentation/Manual/CommandLineArguments.html
            commandLine = new CommandLineBuilder()
                .AppendIf(launchOptions.IsBorderless, "-popupwindow")
                .AppendIf(launchOptions.IsExclusive, "-window-mode", "exclusive")
                .Append("-screen-fullscreen", launchOptions.IsFullScreen ? 1 : 0)
                .AppendIf(launchOptions.IsScreenWidthEnabled, "-screen-width", launchOptions.ScreenWidth)
                .AppendIf(launchOptions.IsScreenHeightEnabled, "-screen-height", launchOptions.ScreenHeight)
                .AppendIf(launchOptions.IsMonitorEnabled, "-monitor", launchOptions.Monitor.Value)
                .AppendIf(launchOptions.IsUseCloudThirdPartyMobile, "-platform_type CLOUD_THIRD_PARTY_MOBILE")
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
}

internal sealed class LaunchExecutionSetDiscordActivityHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        IDiscordService discordService = context.ServiceProvider.GetRequiredService<IDiscordService>();
        bool previousSetDiscordActivityWhenPlaying = context.Options.SetDiscordActivityWhenPlaying;
        if (previousSetDiscordActivityWhenPlaying)
        {
            await discordService.SetPlayingActivityAsync(context.Scheme.IsOversea).ConfigureAwait(false);
        }

        await next().ConfigureAwait(false);

        if (previousSetDiscordActivityWhenPlaying)
        {
            await discordService.SetNormalActivityAsync().ConfigureAwait(false);
        }
    }
}