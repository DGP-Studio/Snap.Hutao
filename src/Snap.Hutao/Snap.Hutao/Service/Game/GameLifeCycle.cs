// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Factory.Process;
using Snap.Hutao.Win32;

namespace Snap.Hutao.Service.Game;

internal static class GameLifeCycle
{
    public static bool IsGameRunning()
    {
        return IsGameRunning(out _);
    }

    public static bool IsGameRunning([NotNullWhen(true)] out IProcess? runningProcess)
    {
        return ProcessFactory.IsRunning([GameConstants.YuanShenProcessName, GameConstants.GenshinImpactProcessName], out runningProcess);
    }

    public static bool TryKillGameProcess()
    {
        if (!IsGameRunning(out IProcess? process))
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
}