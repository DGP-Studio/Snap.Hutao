// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class YaeLaunchExecutionGameProcessInitializationHandler : ILaunchExecutionDelegateHandler
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

        context.Progress.Report(new(LaunchPhase.ProcessInitializing, SH.ServiceGameLaunchPhaseProcessInitializing));
        using (context.Process = InitializeGameProcess(context, gameFileSystem))
        {
            await next().ConfigureAwait(false);
        }
    }

    private static System.Diagnostics.Process InitializeGameProcess(LaunchExecutionContext context, IGameFileSystemView gameFileSystem)
    {
        LaunchOptions launchOptions = context.Options;

        bool authTicketAvailable = launchOptions is { AreCommandLineArgumentsEnabled: true, UsingHoyolabAccount: true } && !string.IsNullOrEmpty(context.AuthTicket);

        // https://docs.unity.cn/cn/current/Manual/PlayerCommandLineArguments.html
        // https://docs.unity3d.com/2017.4/Documentation/Manual/CommandLineArguments.html
        string commandLine = new CommandLineBuilder()
            .Append("-screen-fullscreen", 0)
            .Append("-screen-width", 800)
            .Append("-screen-height", 450)
            .AppendIf(authTicketAvailable, "login_auth_ticket", context.AuthTicket, CommandLineArgumentPrefix.Equal)
            .ToString();

        context.Logger.LogInformation("Command Line Arguments: {commandLine}", commandLine);

        return new()
        {
            StartInfo = new()
            {
                Arguments = commandLine,
                FileName = gameFileSystem.GameFilePath,
                UseShellExecute = true,
                Verb = "runas",
                WorkingDirectory = gameFileSystem.GetGameDirectory(),
            },
        };
    }
}