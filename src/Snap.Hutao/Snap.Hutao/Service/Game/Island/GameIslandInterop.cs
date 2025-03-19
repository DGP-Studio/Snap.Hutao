// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Feature;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Service.Game.Island;

internal sealed class GameIslandInterop : IGameIslandInterop
{
    private const string IslandEnvironmentName = "4F3E8543-40F7-4808-82DC-21E48A6037A7";

    private readonly LaunchExecutionContext context;
    private readonly bool resume;
    private readonly string dataFolderIslandPath;

    private IslandFunctionOffsets offsets;
    private int accumulatedBadStateCount;

    public GameIslandInterop(LaunchExecutionContext context, bool resume)
    {
        this.context = context;
        this.resume = resume;
        dataFolderIslandPath = Path.Combine(HutaoRuntime.DataFolder, "Snap.Hutao.UnlockerIsland.dll");
    }

    public async ValueTask<bool> PrepareAsync(CancellationToken token = default)
    {
        if (!context.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
        {
            return false;
        }

        if (!gameFileSystem.TryGetGameVersion(out string? gameVersion))
        {
            return false;
        }

        IFeatureService featureService = context.ServiceProvider.GetRequiredService<IFeatureService>();
        if (await featureService.GetIslandFeatureAsync(gameVersion).ConfigureAwait(false) is not { } feature)
        {
            return false;
        }

        offsets = context.TargetScheme.IsOversea ? feature.Oversea : feature.Chinese;

        DebugReplaceOffsets(ref offsets);

        if (!resume)
        {
#if !PREVENT_COPY_ISLAND_DLL
            try
            {
                InstalledLocation.CopyFileFromApplicationUri("ms-appx:///Snap.Hutao.UnlockerIsland.dll", dataFolderIslandPath);
            }
            catch
            {
                context.Logger.LogError("Failed to copy island file.");
                throw;
            }
#endif
        }

        return true;
    }

    public async ValueTask WaitForExitAsync(CancellationToken token = default)
    {
        try
        {
            using (MemoryMappedFile file = MemoryMappedFile.CreateOrOpen(IslandEnvironmentName, 1024))
            {
                using (MemoryMappedViewAccessor accessor = file.CreateViewAccessor())
                {
                    nint handle = accessor.SafeMemoryMappedViewHandle.DangerousGetHandle();
                    InitializeIslandEnvironment(handle, offsets, context.Options);
                    if (!resume)
                    {
                        InitializeIsland(context.Process);
                    }

                    using (PeriodicTimer timer = new(TimeSpan.FromMilliseconds(500)))
                    {
                        while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
                        {
                            if (context.Process.HasExited)
                            {
                                break;
                            }

                            IslandEnvironmentView view = UpdateIslandEnvironment(handle, context.Options);

                            if (view.State is IslandState.None or IslandState.Stopped)
                            {
                                if (Interlocked.Increment(ref accumulatedBadStateCount) >= 10)
                                {
                                    if (resume)
                                    {
                                        // https://github.com/DGP-Studio/Snap.Hutao/issues/2540
                                        // Simply return if the game is running without island injected previously
                                        return;
                                    }

                                    HutaoException.Throw($"UnlockerIsland in bad state for too long, last state: {view.State}");
                                }
                            }
                            else
                            {
                                Interlocked.Exchange(ref accumulatedBadStateCount, 0);
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            context.Logger.LogInformation("Exit PostUnlockAsync");
        }
    }

    [Conditional("DEBUG")]
    private static void DebugReplaceOffsets(ref IslandFunctionOffsets offsets)
    {
        // offsets.OpenTeamPageAccordingly = 0x07CB71C0;
    }

    private static unsafe void InitializeIslandEnvironment(nint handle, IslandFunctionOffsets offsets, LaunchOptions options)
    {
        IslandEnvironment* pIslandEnvironment = (IslandEnvironment*)handle;

        pIslandEnvironment->FunctionOffsets = offsets;
        pIslandEnvironment->HookingSetFieldOfView = options.HookingSetFieldOfView;
        pIslandEnvironment->HookingOpenTeam = options.HookingOpenTeam;
        pIslandEnvironment->HookingMickyWonderPartner2 = options.HookingMickyWonderPartner2;
        pIslandEnvironment->HookingSetupQuestBanner = options.HookingSetupQuestBanner;
        pIslandEnvironment->HookingEventCameraMove = options.HookingEventCameraMove;

        UpdateIslandEnvironment(handle, options);
    }

    private static unsafe IslandEnvironmentView UpdateIslandEnvironment(nint handle, LaunchOptions options)
    {
        IslandEnvironment* pIslandEnvironment = (IslandEnvironment*)handle;

        pIslandEnvironment->EnableSetFieldOfView = options.IsSetFieldOfViewEnabled;
        pIslandEnvironment->FieldOfView = options.TargetFov;
        pIslandEnvironment->FixLowFovScene = options.FixLowFovScene;
        pIslandEnvironment->DisableFog = options.DisableFog;
        pIslandEnvironment->EnableSetTargetFrameRate = options.IsSetTargetFrameRateEnabled;
        pIslandEnvironment->TargetFrameRate = options.TargetFps;
        pIslandEnvironment->RemoveOpenTeamProgress = options.RemoveOpenTeamProgress;
        pIslandEnvironment->HideQuestBanner = options.HideQuestBanner;
        pIslandEnvironment->DisableEventCameraMove = options.DisableEventCameraMove;

        return *(IslandEnvironmentView*)pIslandEnvironment;
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
            HHOOK hHook = SetWindowsHookExW(WINDOWS_HOOK_ID.WH_GETMESSAGE, hookProc, hModule, threadId);
            if (hHook.Value is 0)
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
                HutaoException.Throw("SetWindowsHookExW returned 'NULL' but no Error is presented");
            }

            if (!PostThreadMessageW(threadId, WM_NULL, default, default))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
                HutaoException.Throw("PostThreadMessageW returned 'FALSE' but no Error is presented");
            }
        }
        finally
        {
            // NEVER UNHOOK: Will cause the dll unload in game process
            // UnhookWindowsHookEx(hHook);
            NativeLibrary.Free(hModule);
        }
    }
}