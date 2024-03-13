// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionEnsureGameNotRunningHandler : ILaunchExecutionDelegateHandler
{
    public static bool IsGameRunning([NotNullWhen(true)] out Process? runningProcess)
    {
        int currentSessionId = Process.GetCurrentProcess().SessionId;

        // GetProcesses once and manually loop is O(n)
        foreach (ref readonly Process process in Process.GetProcesses().AsSpan())
        {
            if (string.Equals(process.ProcessName, GameConstants.YuanShenProcessName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(process.ProcessName, GameConstants.GenshinImpactProcessName, StringComparison.OrdinalIgnoreCase))
            {
                if (process.SessionId != currentSessionId)
                {
                    continue;
                }

                runningProcess = process;
                return true;
            }
        }

        runningProcess = default;
        return false;
    }

    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (IsGameRunning(out Process? process))
        {
            context.Logger.LogInformation("Game process detected, id: {Id}", process.Id);

            context.Result.Kind = LaunchExecutionResultKind.GameProcessRunning;
            context.Result.ErrorMessage = SH.ServiceGameLaunchExecutionGameIsRunning;
            return;
        }

        await next().ConfigureAwait(false);
    }
}