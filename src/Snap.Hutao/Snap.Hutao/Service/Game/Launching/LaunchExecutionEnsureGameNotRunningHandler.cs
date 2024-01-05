// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class LaunchExecutionEnsureGameNotRunningHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (IsGameRunning())
        {
            context.Result.Kind = LaunchExecutionResultKind.GameProcessRunning;
            context.Result.ErrorMessage = SH.ServiceGameLaunchExecutionGameIsRunning;
            return;
        }

        await next().ConfigureAwait(false);
    }

    private static bool IsGameRunning()
    {
        // GetProcesses once and manually loop is O(n)
        foreach (ref readonly System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses().AsSpan())
        {
            if (string.Equals(process.ProcessName, GameConstants.YuanShenProcessName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(process.ProcessName, GameConstants.GenshinImpactProcessName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}