// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Factory.Process;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Launching.Context;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class GameProcessFactory
{
    public static IProcess CreateForDefault(BeforeLaunchExecutionContext context)
    {
        LaunchOptions launchOptions = context.LaunchOptions;

        string commandLine = string.Empty;
        if (launchOptions.AreCommandLineArgumentsEnabled.Value)
        {
            string? authTicket = default;
            bool useAuthTicket = launchOptions.UsingHoyolabAccount.Value
                && context.TryGetOption(LaunchExecutionOptionsKey.LoginAuthTicket, out authTicket)
                && !string.IsNullOrEmpty(authTicket);

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
                .AppendIf(useAuthTicket, "login_auth_ticket", authTicket, CommandLineArgumentPrefix.Equal)
                .ToString();

            context.TaskContext.InvokeOnMainThread(() =>
            {
                launchOptions.AspectRatios.Add(new(launchOptions.ScreenWidth.Value, launchOptions.ScreenHeight.Value));
            });
        }

        string gameFilePath = context.FileSystem.GameFilePath;
        string gameDirectory = context.FileSystem.GetGameDirectory();

        return HutaoRuntime.IsProcessElevated && launchOptions.IsIslandEnabled.Value
            ? ProcessFactory.CreateSuspended(commandLine, gameFilePath, gameDirectory)
            : ProcessFactory.CreateUsingShellExecuteRunAs(commandLine, gameFilePath, gameDirectory);
    }

    public static IProcess CreateForEmbeddedYae(BeforeLaunchExecutionContext context)
    {
        LaunchOptions launchOptions = context.LaunchOptions;

        string? authTicket = default;
        bool useAuthTicket = launchOptions.AreCommandLineArgumentsEnabled.Value
            && launchOptions.UsingHoyolabAccount.Value
            && context.TryGetOption(LaunchExecutionOptionsKey.LoginAuthTicket, out authTicket)
            && !string.IsNullOrEmpty(authTicket);

        string commandLine = new CommandLineBuilder()
            .Append("-screen-fullscreen", 0)
            .Append("-screen-width", 800)
            .Append("-screen-height", 450)
            .AppendIf(useAuthTicket, "login_auth_ticket", authTicket, CommandLineArgumentPrefix.Equal)
            .ToString();

        return ProcessFactory.CreateSuspended(commandLine, context.FileSystem.GameFilePath, context.FileSystem.GetGameDirectory());
    }
}