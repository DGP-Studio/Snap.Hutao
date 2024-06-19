// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.LibraryLoader;
using Snap.Hutao.Win32.System.Threading;
using System.Diagnostics;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 游戏帧率解锁器
/// Credit to https://github.com/34736384/genshin-fps-unlock
/// </summary>
[HighQuality]
internal sealed class GameFpsUnlocker : IGameFpsUnlocker
{
    private readonly LaunchOptions launchOptions;
    private readonly GameFpsUnlockerContext context = new();

    public GameFpsUnlocker(IServiceProvider serviceProvider, Process gameProcess, in UnlockOptions options, IProgress<GameFpsUnlockerContext> progress)
    {
        launchOptions = serviceProvider.GetRequiredService<LaunchOptions>();

        context.GameProcess = gameProcess;
        context.AllAccess = OpenProcess(PROCESS_ACCESS_RIGHTS.PROCESS_ALL_ACCESS, false, (uint)gameProcess.Id);
        context.Options = options;
        context.Progress = progress;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> UnlockAsync(CancellationToken token = default)
    {
        HutaoException.ThrowIfNot(context.IsUnlockerValid, "This Unlocker is invalid");
        (FindModuleResult result, RequiredRemoteModule remoteModule) = await GameProcessModule.FindModuleAsync(context).ConfigureAwait(false);
        HutaoException.ThrowIfNot(result != FindModuleResult.TimeLimitExeeded, SH.ServiceGameUnlockerFindModuleTimeLimitExeeded);
        HutaoException.ThrowIfNot(result != FindModuleResult.NoModuleFound, SH.ServiceGameUnlockerFindModuleNoModuleFound);

        using (RequiredLocalModule localModule = LoadRequiredLocalModule(context.Options.GameFileSystem))
        {
            GameFpsAddress.UnsafeFindFpsAddress(context, remoteModule, localModule);
        }

        context.Report();
        return context.FpsAddress != 0U;
    }

    public async ValueTask PostUnlockAsync(CancellationToken token = default)
    {
        using (PeriodicTimer timer = new(context.Options.AdjustFpsDelay))
        {
            while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
            {
                if (!context.GameProcess.HasExited && context.FpsAddress != 0U)
                {
                    UnsafeWriteProcessMemory(context.GameProcess, context.FpsAddress, launchOptions.TargetFps);
                    WIN32_ERROR error = GetLastError();
                    if (error is not WIN32_ERROR.NO_ERROR)
                    {
                        context.Description = SH.FormatServiceGameUnlockerWriteProcessMemoryFpsAddressFailed(error);
                    }

                    context.Report();
                }
                else
                {
                    context.IsUnlockerValid = false;
                    context.FpsAddress = 0;
                    context.Report();
                    return;
                }
            }
        }
    }

    private static unsafe bool UnsafeWriteProcessMemory(System.Diagnostics.Process process, nuint baseAddress, int value)
    {
        return WriteProcessMemory((Win32.Foundation.HANDLE)process.Handle, (void*)baseAddress, ref value, out _);
    }

    private static RequiredLocalModule LoadRequiredLocalModule(GameFileSystem gameFileSystem)
    {
        string gameFoler = gameFileSystem.GameDirectory;
        string dataFoler = gameFileSystem.DataDirectory;
        LOAD_LIBRARY_FLAGS flags = LOAD_LIBRARY_FLAGS.LOAD_LIBRARY_AS_IMAGE_RESOURCE;
        HMODULE unityPlayerAddress = LoadLibraryExW(System.IO.Path.Combine(gameFoler, "UnityPlayer.dll"), default, flags);
        HMODULE userAssemblyAddress = LoadLibraryExW(System.IO.Path.Combine(dataFoler, "Native", "UserAssembly.dll"), default, flags);

        return new(unityPlayerAddress, userAssemblyAddress);
    }
}