// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Feature;
using Snap.Hutao.Service.Game.Unlocker.Island;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Memory;
using Snap.Hutao.Win32.System.Threading;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
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
    private readonly string gameVersion;

    private IslandFunctionOffsets? offsets;
    private int accumulatedBadStateCount;

    public GameFpsUnlocker(IServiceProvider serviceProvider, Process gameProcess, string gameVersion)
    {
        launchOptions = serviceProvider.GetRequiredService<LaunchOptions>();
        featureService = serviceProvider.GetRequiredService<IFeatureService>();

        RuntimeOptions runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();
        dataFolderIslandPath = Path.Combine(runtimeOptions.DataFolder, "Snap.Hutao.UnlockerIsland.dll");

        this.gameVersion = gameVersion;

        context.GameProcess = gameProcess;
        context.Logger = serviceProvider.GetRequiredService<ILogger<GameFpsUnlocker>>();
    }

    public async ValueTask<bool> UnlockAsync(CancellationToken token = default)
    {
        HutaoException.ThrowIfNot(context.IsUnlockerValid, "This Unlocker is invalid");

        if (await featureService.GetIslandFeatureAsync(gameVersion).ConfigureAwait(false) is not { } feature)
        {
            return false;
        }

        offsets = string.Equals(GameConstants.GenshinImpactProcessName, context.GameProcess.ProcessName, StringComparison.OrdinalIgnoreCase)
            ? feature.Oversea
            : feature.Chinese;

        try
        {
            InstalledLocation.CopyFileFromApplicationUri("ms-appx:///Snap.Hutao.UnlockerIsland.dll", dataFolderIslandPath);
        }
        catch
        {
            context.Logger.LogError("Failed to copy island file.");
            throw;
        }

        return true;
    }

    public async ValueTask PostUnlockAsync(CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(offsets);

        try
        {
            using (MemoryMappedFile file = MemoryMappedFile.CreateOrOpen(IslandEnvironmentName, 1024))
            {
                using (MemoryMappedViewAccessor accessor = file.CreateViewAccessor())
                {
                    nint handle = accessor.SafeMemoryMappedViewHandle.DangerousGetHandle();
                    InitializeIslandEnvironment(handle, offsets, launchOptions);
                    InitializeIsland2(context.GameProcess, Encoding.UTF8.GetBytes(dataFolderIslandPath).AsSpan());
                    using (PeriodicTimer timer = new(TimeSpan.FromMilliseconds(500)))
                    {
                        while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
                        {
                            if (context.GameProcess.HasExited)
                            {
                                break;
                            }

                            IslandEnvironmentView view = UpdateIslandEnvironment(handle, launchOptions);
                            context.Logger.LogDebug("Island Environment|{State}|{Error}|{Value}", view.State, view.LastError, view.DebugOriginalFieldOfView);

                            if (view.State is IslandState.None or IslandState.Stopped)
                            {
                                if (Interlocked.Increment(ref accumulatedBadStateCount) >= 5)
                                {
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

    private static unsafe void InitializeIslandEnvironment(nint handle, IslandFunctionOffsets offsets, LaunchOptions options)
    {
        IslandEnvironment* pIslandEnvironment = (IslandEnvironment*)handle;
        pIslandEnvironment->FunctionOffsetSetFieldOfView = offsets.FunctionOffsetSetFieldOfView;
        pIslandEnvironment->FunctionOffsetSetTargetFrameRate = offsets.FunctionOffsetSetTargetFrameRate;
        pIslandEnvironment->FunctionOffsetSetEnableFogRendering = offsets.FunctionOffsetSetEnableFogRendering;

        pIslandEnvironment->LoopAdjustFpsOnly = options.LoopAdjustFpsOnly;

        UpdateIslandEnvironment(handle, options);
    }

    private static unsafe IslandEnvironmentView UpdateIslandEnvironment(nint handle, LaunchOptions options)
    {
        IslandEnvironment* pIslandEnvironment = (IslandEnvironment*)handle;
        pIslandEnvironment->FieldOfView = options.TargetFov;
        pIslandEnvironment->TargetFrameRate = options.TargetFps;
        pIslandEnvironment->DisableFog = options.DisableFog;

        return *(IslandEnvironmentView*)pIslandEnvironment;
    }

    [Obsolete]
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
            NativeLibrary.Free(hModule);
        }
    }

    private unsafe void InitializeIsland2(Process gameProcess, ReadOnlySpan<byte> libraryPath)
    {
        HMODULE hKernel32 = GetModuleHandleW("kernel32.dll");
        if (hKernel32 == default)
        {
            Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            HutaoException.Throw("GetModuleHandleW returned 'NULL' but no Error is presented");
        }

        nint lpLoadLibraryW = NativeLibrary.GetExport(hKernel32, "LoadLibraryW");
        if (lpLoadLibraryW == default)
        {
            Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            HutaoException.Throw("GetProcAddress returned 'NULL' but no Error is presented");
        }

        HANDLE hProcess = gameProcess.Handle;
        void* lpBase = VirtualAllocEx(hProcess, default, (uint)libraryPath.Length + 1, VIRTUAL_ALLOCATION_TYPE.MEM_RESERVE | VIRTUAL_ALLOCATION_TYPE.MEM_COMMIT, PAGE_PROTECTION_FLAGS.PAGE_READWRITE);
        if (lpBase is null)
        {
            Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            HutaoException.Throw("VirtualAllocEx returned 'NULL' but no Error is presented");
        }

        try
        {
            if (!WriteProcessMemory(hProcess, lpBase, libraryPath, out _))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
                HutaoException.Throw("WriteProcessMemory returned 'FALSE' but no Error is presented");
            }

            LPTHREAD_START_ROUTINE lpStartAddress = LPTHREAD_START_ROUTINE.Create((delegate* unmanaged[Stdcall]<void*, uint>)(delegate* unmanaged[Stdcall]<PCWSTR, HMODULE>)lpLoadLibraryW);
            HANDLE hThread = CreateRemoteThread(hProcess, default, 0, lpStartAddress, lpBase, 0, out _);
            if (hThread == default)
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
                HutaoException.Throw("CreateRemoteThread returned 'NULL' but no Error is presented");
            }

            WaitForSingleObject(hThread, 2000);
            if (!CloseHandle(hThread))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
                HutaoException.Throw("CloseHandle returned 'FALSE' but no Error is presented");
            }
        }
        finally
        {
            VirtualFreeEx(hProcess, lpBase, 0, VIRTUAL_FREE_TYPE.MEM_RELEASE);
        }
    }
}