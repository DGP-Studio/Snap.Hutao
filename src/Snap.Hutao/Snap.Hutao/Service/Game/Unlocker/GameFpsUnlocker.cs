// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32.Foundation;
using Windows.Win32.System.Diagnostics.ToolHelp;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 游戏帧率解锁器
/// Credit to https://github.com/34736384/genshin-fps-unlock
/// </summary>
[HighQuality]
internal sealed class GameFpsUnlocker : IGameFpsUnlocker
{
    private readonly Process gameProcess;
    private readonly LaunchOptions launchOptions;
    private readonly ILogger<GameFpsUnlocker> logger;

    private nuint fpsAddress;
    private bool isValid = true;

    /// <summary>
    /// 构造一个新的 <see cref="GameFpsUnlocker"/> 对象，
    /// 每个解锁器只能解锁一次原神的进程，
    /// 再次解锁需要重新创建对象
    /// <para/>
    /// 解锁器需要在管理员模式下才能正确的完成解锁操作，
    /// 非管理员模式不能解锁
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="gameProcess">游戏进程</param>
    public GameFpsUnlocker(IServiceProvider serviceProvider, Process gameProcess)
    {
        launchOptions = serviceProvider.GetRequiredService<LaunchOptions>();
        logger = serviceProvider.GetRequiredService<ILogger<GameFpsUnlocker>>();
        this.gameProcess = gameProcess;
    }

    /// <inheritdoc/>
    public async Task UnlockAsync(UnlockTimingOptions options)
    {
        Verify.Operation(isValid, "This Unlocker is invalid");

        GameModuleEntryInfo moduleEntryInfo = await FindModuleAsync(options.FindModuleDelay, options.FindModuleLimit).ConfigureAwait(false);
        Must.Argument(moduleEntryInfo.HasValue, "读取游戏内存失败");

        // Read UnityPlayer.dll
        UnsafeTryReadModuleMemoryFindFpsAddress(moduleEntryInfo);

        // When player switch between scenes, we have to re adjust the fps
        // So we keep a loop here
        await LoopAdjustFpsAsync(options.AdjustFpsDelay).ConfigureAwait(false);
    }

    private static unsafe bool UnsafeReadModulesMemory(Process process, in GameModuleEntryInfo moduleEntryInfo, out VirtualMemory memory)
    {
        MODULEENTRY32 unityPlayer = moduleEntryInfo.UnityPlayer;
        MODULEENTRY32 userAssembly = moduleEntryInfo.UserAssembly;

        memory = new VirtualMemory(unityPlayer.modBaseSize + userAssembly.modBaseSize);
        byte* lpBuffer = (byte*)memory.Pointer;
        return ReadProcessMemory((HANDLE)process.Handle, unityPlayer.modBaseAddr, lpBuffer, unityPlayer.modBaseSize, default)
            && ReadProcessMemory((HANDLE)process.Handle, userAssembly.modBaseAddr, lpBuffer + unityPlayer.modBaseSize, userAssembly.modBaseSize, default);
    }

    private static unsafe bool UnsafeReadProcessMemory(Process process, nuint baseAddress, out nuint value)
    {
        ulong temp = 0;
        bool result = ReadProcessMemory((HANDLE)process.Handle, (void*)baseAddress, (byte*)&temp, 8, default);
        if (!result)
        {
            ThrowHelper.InvalidOperation("读取进程内存失败", null);
        }

        value = (nuint)temp;
        return result;
    }

    private static unsafe bool UnsafeWriteProcessMemory(Process process, nuint baseAddress, int write)
    {
        return WriteProcessMemory((HANDLE)process.Handle, (void*)baseAddress, &write, sizeof(int), default);
    }

    private static unsafe MODULEENTRY32 UnsafeFindModule(int processId, in ReadOnlySpan<byte> moduleName)
    {
        HANDLE snapshot = CreateToolhelp32Snapshot(CREATE_TOOLHELP_SNAPSHOT_FLAGS.TH32CS_SNAPMODULE, (uint)processId);
        try
        {
            Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            foreach (MODULEENTRY32 entry in StructMarshal.EnumerateModuleEntry32(snapshot))
            {
                ReadOnlySpan<byte> szModuleLocal = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)&entry.szModule);
                if (entry.th32ProcessID == processId && szModuleLocal.SequenceEqual(moduleName))
                {
                    return entry;
                }
            }

            return default;
        }
        finally
        {
            CloseHandle(snapshot);
        }
    }

    private static int IndexOfPattern(in ReadOnlySpan<byte> memory)
    {
        // E8 ?? ?? ?? ?? 85 C0 7E 07 E8 ?? ?? ?? ?? EB 05
        int second = 0;
        ReadOnlySpan<byte> secondPart = new byte[] { 0x85, 0xC0, 0x7E, 0x07, 0xE8, };
        ReadOnlySpan<byte> thirdPart = new byte[] { 0xEB, 0x05, };

        while (second >= 0 && second < memory.Length)
        {
            second += memory[second..].IndexOf(secondPart);
            if (memory[second - 5].Equals(0xE8) && memory.Slice(second + 9, 2).SequenceEqual(thirdPart))
            {
                return second - 5;
            }

            second += 5;
        }

        return -1;
    }

    private static unsafe GameModuleEntryInfo UnsafeGetGameModuleEntryInfo(int processId)
    {
        MODULEENTRY32 unityPlayer = UnsafeFindModule(processId, "UnityPlayer.dll"u8);
        MODULEENTRY32 userAssembly = UnsafeFindModule(processId, "UserAssembly.dll"u8);

        if (unityPlayer.modBaseSize != 0 && userAssembly.modBaseSize != 0)
        {
            return new(unityPlayer, userAssembly);
        }

        return default;
    }

    private async Task<GameModuleEntryInfo> FindModuleAsync(TimeSpan findModuleDelay, TimeSpan findModuleLimit)
    {
        ValueStopwatch watch = ValueStopwatch.StartNew();
        using (PeriodicTimer timer = new(findModuleDelay))
        {
            while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                GameModuleEntryInfo moduleInfo = UnsafeGetGameModuleEntryInfo(gameProcess.Id);
                if (moduleInfo.HasValue)
                {
                    return moduleInfo;
                }

                if (watch.GetElapsedTime() > findModuleLimit)
                {
                    break;
                }
            }
        }

        return default;
    }

    private async Task LoopAdjustFpsAsync(TimeSpan adjustFpsDelay)
    {
        using (PeriodicTimer timer = new(adjustFpsDelay))
        {
            while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                if (!gameProcess.HasExited && fpsAddress != 0)
                {
                    UnsafeWriteProcessMemory(gameProcess, fpsAddress, launchOptions.TargetFps);
                }
                else
                {
                    isValid = false;
                    fpsAddress = 0;
                    return;
                }
            }
        }
    }

    private unsafe void UnsafeTryReadModuleMemoryFindFpsAddress(in GameModuleEntryInfo moduleEntryInfo)
    {
        bool readOk = UnsafeReadModulesMemory(gameProcess, moduleEntryInfo, out VirtualMemory localMemory);
        Verify.Operation(readOk, "读取内存失败");

        using (localMemory)
        {
            int offset = IndexOfPattern(localMemory.GetBuffer()[(int)moduleEntryInfo.UnityPlayer.modBaseSize..]);
            Must.Range(offset > 0, "未匹配到FPS字节");

            byte* pLocalMemory = (byte*)localMemory.Pointer;
            MODULEENTRY32 unityPlayer = moduleEntryInfo.UnityPlayer;
            MODULEENTRY32 userAssembly = moduleEntryInfo.UserAssembly;

            nuint localMemoryUnityPlayerAddress = (nuint)pLocalMemory;
            nuint localMemoryUserAssemblyAddress = localMemoryUnityPlayerAddress + unityPlayer.modBaseSize;

            nuint rip = localMemoryUserAssemblyAddress + (uint)offset;
            rip += *(uint*)(rip + 1) + 5;
            rip += *(uint*)(rip + 3) + 7;

            nuint address = (nuint)userAssembly.modBaseAddr + (rip - localMemoryUserAssemblyAddress);
            logger.LogInformation("Game Process handle: {handle}", gameProcess.Handle);

            nuint ptr = 0;
            SpinWait.SpinUntil(() => UnsafeReadProcessMemory(gameProcess, address, out ptr) && ptr != 0);

            logger.LogInformation("UnsafeReadProcessMemory succeed {addr:x8}", ptr);
            rip = ptr - (nuint)unityPlayer.modBaseAddr + localMemoryUnityPlayerAddress;
            logger.LogInformation("UnityPlayer addr: {up:x8}, rip addr: {rip:x8}", localMemoryUnityPlayerAddress, rip);
            while (*(byte*)rip == 0xE8 || *(byte*)rip == 0xE9)
            {
                rip += (nuint)(*(int*)(rip + 1) + 5);
                logger.LogInformation("rip ? addr: {rip:x8}", rip);
            }

            nuint localMemoryActualAddress = rip + *(uint*)(rip + 2) + 6;
            nuint actualOffset = localMemoryActualAddress - localMemoryUnityPlayerAddress;
            fpsAddress = (nuint)(unityPlayer.modBaseAddr + actualOffset);
            logger.LogInformation("UnsafeTryReadModuleMemoryFindFpsAddress finished");
        }
    }

    private readonly struct GameModuleEntryInfo
    {
        public readonly bool HasValue = false;
        public readonly MODULEENTRY32 UnityPlayer;
        public readonly MODULEENTRY32 UserAssembly;

        public GameModuleEntryInfo(MODULEENTRY32 unityPlayer, MODULEENTRY32 userAssembly)
        {
            HasValue = true;
            UnityPlayer = unityPlayer;
            UserAssembly = userAssembly;
        }
    }
}