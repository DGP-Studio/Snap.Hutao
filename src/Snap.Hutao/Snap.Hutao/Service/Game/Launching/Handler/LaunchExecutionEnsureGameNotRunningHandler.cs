// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32;
using System.Diagnostics;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionEnsureGameNotRunningHandler : ILaunchExecutionDelegateHandler
{
    public static bool IsGameRunning()
    {
        return IsGameRunning(out _);
    }

    public static bool IsGameRunning([NotNullWhen(true)] out Process? runningProcess)
    {
        int currentSessionId = Process.GetCurrentProcess().SessionId;

        // GetProcesses once and manually loop is O(n)
        foreach (ref readonly Process process in Process.GetProcesses().AsSpan())
        {
            if (!process.ProcessName.EqualsAny([GameConstants.YuanShenProcessName, GameConstants.GenshinImpactProcessName], StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (process.SessionId != currentSessionId)
            {
                continue;
            }

            runningProcess = process;
            return true;
        }

        runningProcess = default;
        return false;
    }

    public static bool TryKillGameProcess()
    {
        if (!IsGameRunning(out Process? process))
        {
            return false;
        }

        try
        {
            process.Kill();
            return true;
        }
        catch (Exception ex)
        {
            HutaoNative.Instance.ShowErrorMessage(SH.ServiceGameLaunchExecutionGameRunningKillFailed, ex.Message);
            return false;
        }
    }

    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (IsGameRunning())
        {
            context.Result.Kind = LaunchExecutionResultKind.GameProcessRunning;
            context.Result.ErrorMessage = SH.ServiceGameLaunchExecutionGameIsRunning;
            return;
        }

        await next().ConfigureAwait(false);
    }
}