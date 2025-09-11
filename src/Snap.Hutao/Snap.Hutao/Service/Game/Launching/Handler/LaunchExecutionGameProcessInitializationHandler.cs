// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Factory.Process;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameProcessInitializationHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (!context.TryGetGameFileSystem(out IGameFileSystemView? gameFileSystem))
        {
            return;
        }

        context.Progress.Report(new(SH.ServiceGameLaunchPhaseProcessInitializing));
        using (context.Process = InitializeGameProcess(context, gameFileSystem))
        {
            await next().ConfigureAwait(false);
        }
    }

    private static IProcess InitializeGameProcess(LaunchExecutionContext context, IGameFileSystemView gameFileSystem)
    {
        LaunchOptions launchOptions = context.Options;

        string commandLine = string.Empty;
        if (launchOptions.AreCommandLineArgumentsEnabled.Value)
        {
            // https://docs.unity.cn/cn/current/Manual/PlayerCommandLineArguments.html
            // https://docs.unity3d.com/2017.4/Documentation/Manual/CommandLineArguments.html
            commandLine = new CommandLineBuilder()
                .AppendIf(launchOptions.IsBorderless.Value, "-popupwindow")
                .AppendIf(launchOptions.IsExclusive.Value, "-window-mode", "exclusive")
                .Append("-screen-fullscreen", launchOptions.IsFullScreen.Value ? "1" : "0")
                .AppendIf(launchOptions.IsScreenWidthEnabled.Value, "-screen-width", launchOptions.ScreenWidth.Value)
                .AppendIf(launchOptions.IsScreenHeightEnabled.Value, "-screen-height", launchOptions.ScreenHeight.Value)
                .AppendIf(launchOptions.IsMonitorEnabled.Value, "-monitor", launchOptions.Monitor.Value?.Value ?? 1)
                .AppendIf(launchOptions.IsPlatformTypeEnabled.Value, "-platform_type", $"{launchOptions.PlatformType.Value:G}")
                .AppendIf(launchOptions.UsingHoyolabAccount.Value && !string.IsNullOrEmpty(context.AuthTicket), "login_auth_ticket", context.AuthTicket, CommandLineArgumentPrefix.Equal)
                .ToString();

            context.TaskContext.InvokeOnMainThread(() =>
            {
                launchOptions.AspectRatios.Add(new(launchOptions.ScreenWidth.Value, launchOptions.ScreenHeight.Value));
            });
        }

        context.Logger.LogInformation("Command Line Arguments: {commandLine}", commandLine);
        return HutaoRuntime.IsProcessElevated && context.Options.IsIslandEnabled.Value
            ? ProcessFactory.CreateSuspended(commandLine, gameFileSystem.GameFilePath, gameFileSystem.GetGameDirectory())
            : ProcessFactory.CreateUsingShellExecuteRunAs(commandLine, gameFileSystem.GameFilePath, gameFileSystem.GetGameDirectory());
    }
}