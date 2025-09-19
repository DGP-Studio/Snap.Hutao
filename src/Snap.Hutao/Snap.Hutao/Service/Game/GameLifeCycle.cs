// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Factory.Process;
using Snap.Hutao.Win32;

namespace Snap.Hutao.Service.Game;

internal static class GameLifeCycle
{
    public static IObservableProperty<bool> IsGameRunningProperty { get; } = Property.CreateObservable(PrivateIsGameRunning(out _));

    public static async ValueTask SpinWaitGameExitAsync(ITaskContext taskContext)
    {
        await taskContext.SwitchToBackgroundAsync();
        unsafe
        {
            SpinWaitPolyfill.SpinWhile(&IsGameRunning);
        }

        await taskContext.SwitchToMainThreadAsync();
        IsGameRunningProperty.Value = false;
    }

    public static bool IsGameRunning()
    {
        bool result = PrivateIsGameRunning(out _);
        IsGameRunningProperty.Value = result;
        return result;
    }

    public static bool IsGameRunning([NotNullWhen(true)] out IProcess? runningProcess)
    {
        bool result = PrivateIsGameRunning(out runningProcess);
        IsGameRunningProperty.Value = result;
        return result;
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
            IsGameRunningProperty.Value = PrivateIsGameRunning(out _);
            return true;
        }
        catch (Exception ex)
        {
            HutaoNative.Instance.ShowErrorMessage(SH.ServiceGameLaunchExecutionGameRunningKillFailed, ex.Message);
            return false;
        }
    }

    private static bool PrivateIsGameRunning([NotNullWhen(true)] out IProcess? runningProcess)
    {
        return ProcessFactory.IsRunning([GameConstants.YuanShenProcessName, GameConstants.GenshinImpactProcessName], out runningProcess);
    }
}