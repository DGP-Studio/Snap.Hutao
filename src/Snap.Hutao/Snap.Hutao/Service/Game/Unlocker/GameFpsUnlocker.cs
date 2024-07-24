// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.AvatarInfo.Factory.Builder;
using Snap.Hutao.Service.Feature;
using Snap.Hutao.Service.Game.Unlocker.Island;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.LibraryLoader;
using Snap.Hutao.Win32.System.Threading;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Service.Game.Unlocker;

internal sealed class GameFpsUnlocker : IGameFpsUnlocker
{
    private const string IslandEnvironmentName = "4F3E8543-40F7-4808-82DC-21E48A6037A7";
    private readonly LaunchOptions launchOptions;
    private readonly IFeatureService featureService;

    private readonly GameFpsUnlockerContext context = new();
    private readonly string dataFolderIslandPath;

    private IslandFunctionOffsets? offsets;

    public GameFpsUnlocker(IServiceProvider serviceProvider, Process gameProcess, IProgress<GameFpsUnlockerContext> progress)
    {
        launchOptions = serviceProvider.GetRequiredService<LaunchOptions>();
        featureService = serviceProvider.GetRequiredService<IFeatureService>();

        RuntimeOptions runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();
        dataFolderIslandPath = Path.Combine(runtimeOptions.DataFolder, "Snap.Hutao.UnlockerIsland.dll");

        context.GameProcess = gameProcess;
        context.Progress = progress;
        context.Logger = serviceProvider.GetRequiredService<ILogger<GameFpsUnlocker>>();
    }

    public async ValueTask<bool> UnlockAsync(CancellationToken token = default)
    {
        HutaoException.ThrowIfNot(context.IsUnlockerValid, "This Unlocker is invalid");
        if (await featureService.GetIslandFeatureAsync().ConfigureAwait(false) is not { } feature)
        {
            return false;
        }

        offsets = string.Equals(GameConstants.GenshinImpactProcessName, context.GameProcess.ProcessName, StringComparison.OrdinalIgnoreCase)
            ? feature.Oversea
            : feature.Chinese;

        context.Report();
        return true;
    }

    public async ValueTask PostUnlockAsync(CancellationToken token = default)
    {
        if (offsets is null)
        {
            return;
        }

        try
        {
            File.Copy(InstalledLocation.GetAbsolutePath("Snap.Hutao.UnlockerIsland.dll"), dataFolderIslandPath, true);
        }
        catch
        {
            context.Logger.LogError("Failed to copy island file.");
            return;
        }

        try
        {
            using (MemoryMappedFile file = MemoryMappedFile.CreateOrOpen(IslandEnvironmentName, 1024))
            {
                using (MemoryMappedViewAccessor accessor = file.CreateViewAccessor())
                {
                    nint handle = accessor.SafeMemoryMappedViewHandle.DangerousGetHandle();
                    InitializeIslandEnvironment(handle, offsets);
                    InitializeIsland(context.GameProcess);
                    await context.GameProcess.WaitForExitAsync().ConfigureAwait(false);
                }
            }
        }
        finally
        {
            context.Logger.LogInformation("Exit PostUnlockAsync");
        }
    }

    private unsafe void InitializeIslandEnvironment(nint handle, IslandFunctionOffsets offsets, LaunchOptions options)
    {
        IslandEnvironment* pIslandEnvironment = (IslandEnvironment*)handle;
        pIslandEnvironment->FunctionOffsetFieldOfView = offsets.FunctionOffsetFieldOfView;
        pIslandEnvironment->FunctionOffsetTargetFrameRate = offsets.FunctionOffsetTargetFrameRate;
        pIslandEnvironment->FunctionOffsetFog = offsets.FunctionOffsetFog;
        pIslandEnvironment->TargetFrameRate = options.TargetFps;
    }

    private unsafe void InitializeIsland(Process gameProcess)
    {
        HANDLE hModule = default;
        try
        {
            hModule = NativeLibrary.Load(dataFolderIslandPath);
            nint pIslandGetWindowHook = NativeLibrary.GetExport((nint)(hModule & ~0x3L), "IslandGetWindowHook");

            HOOKPROC hookProc = default;
            ((delegate* unmanaged[Stdcall]<HOOKPROC*, HRESULT>)pIslandGetWindowHook)(&hookProc);

            SpinWait.SpinUntil(() => gameProcess.MainWindowHandle is not 0);
            uint threadId = GetWindowThreadProcessId(gameProcess.MainWindowHandle, default);
            HHOOK hHook = SetWindowsHookExW(WINDOWS_HOOK_ID.WH_GETMESSAGE, hookProc, (HINSTANCE)hModule, threadId);
            if (hHook.Value is 0)
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            }

            if (!PostThreadMessageW(threadId, WM_NULL, default, default))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            }
        }
        finally
        {
            NativeLibrary.Free(hModule);
        }
    }
}