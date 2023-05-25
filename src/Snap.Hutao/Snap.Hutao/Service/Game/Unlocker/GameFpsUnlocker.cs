// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32.Foundation;
using Windows.Win32.System.Diagnostics.ToolHelp;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 游戏帧率解锁器
/// Credit to https://github.com/34736384/genshin-fps-unlock
/// 
/// TODO: Save memory alloc on GameModuleEntryInfo
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
    public async Task UnlockAsync(TimeSpan findModuleDelay, TimeSpan findModuleLimit, TimeSpan adjustFpsDelay)
    {
        logger.LogInformation("UnlockAsync called");
        Verify.Operation(isValid, "This Unlocker is invalid");

        GameModuleEntryInfo moduleEntryInfo = await FindModuleAsync(findModuleDelay, findModuleLimit).ConfigureAwait(false);
        Must.Argument(moduleEntryInfo.HasValue, "读取游戏内存失败");

        // Read UnityPlayer.dll
        UnsafeTryReadModuleMemoryFindFpsAddress(moduleEntryInfo);

        // When player switch between scenes, we have to re adjust the fps
        // So we keep a loop here
        await LoopAdjustFpsAsync(adjustFpsDelay).ConfigureAwait(false);
    }

    private static unsafe bool UnsafeReadModulesMemory(Process process, GameModuleEntryInfo moduleEntryInfo, out VirtualMemory memory)
    {
        MODULEENTRY32 unityPlayer = moduleEntryInfo.UnityPlayer;
        MODULEENTRY32 userAssembly = moduleEntryInfo.UserAssembly;

        memory = new VirtualMemory(unityPlayer.modBaseSize + userAssembly.modBaseSize);
        byte* lpBuffer = (byte*)memory.Pointer;
        return ReadProcessMemory((HANDLE)process.Handle, unityPlayer.modBaseAddr, lpBuffer, unityPlayer.modBaseSize, default)
            && ReadProcessMemory((HANDLE)process.Handle, userAssembly.modBaseAddr, lpBuffer + unityPlayer.modBaseSize, userAssembly.modBaseSize, default);
    }

    private static unsafe bool UnsafeReadProcessMemory(Process process, nuint offset, out nuint value)
    {
        fixed (nuint* pValue = &value)
        {
            return ReadProcessMemory((HANDLE)process.Handle, (void*)offset, &pValue, unchecked((uint)sizeof(nuint)));
        }
    }

    private static unsafe bool UnsafeWriteModuleMemory(Process process, nuint baseAddress, int write)
    {
        return WriteProcessMemory((HANDLE)process.Handle, (void*)baseAddress, &write, sizeof(int), null);
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

    private static int FindPatternOffsetImplmentation(ReadOnlySpan<byte> memory)
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

    private unsafe GameModuleEntryInfo UnsafeGetGameModuleEntryInfo(int processId)
    {
        logger.LogInformation("UnsafeGetGameModuleEntryInfo called");

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
        logger.LogInformation("FindModuleAsync called");

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
        logger.LogInformation("LoopAdjustFpsAsync called");

        using (PeriodicTimer timer = new(adjustFpsDelay))
        {
            while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                if (!gameProcess.HasExited && fpsAddress != 0)
                {
                    UnsafeWriteModuleMemory(gameProcess, fpsAddress, launchOptions.TargetFps);
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

    private unsafe void UnsafeTryReadModuleMemoryFindFpsAddress(GameModuleEntryInfo moduleEntryInfo)
    {
        logger.LogInformation("UnsafeTryReadModuleMemoryFindFpsAddress called");

        bool readOk = UnsafeReadModulesMemory(gameProcess, moduleEntryInfo, out VirtualMemory localMemory);
        Verify.Operation(readOk, "读取内存失败");

        using (localMemory)
        {
            int offset = FindPatternOffsetImplmentation(localMemory.GetBuffer().Slice(unchecked((int)moduleEntryInfo.UnityPlayer.modBaseSize)));
            Must.Range(offset > 0, "未匹配到FPS字节");

            byte* pLocalMemory = (byte*)localMemory.Pointer;
            MODULEENTRY32 unityPlayer = moduleEntryInfo.UnityPlayer;
            MODULEENTRY32 userAssembly = moduleEntryInfo.UserAssembly;

            logger.LogInformation("Pattern: {bytes}", BitConverter.ToString(localMemory.GetBuffer().Slice((int)(offset + unityPlayer.modBaseSize), 16).ToArray())); //

            nuint localMemoryUnityPlayerAddress = (nuint)pLocalMemory;
            nuint localMemoryUserAssemblyAddress = localMemoryUnityPlayerAddress + unityPlayer.modBaseSize;
            {
                logger.LogInformation("localMemoryUnityPlayerAddress {addr:X8}", localMemoryUnityPlayerAddress);
                logger.LogInformation("localMemoryUserAssemblyAddress {addr:X8}", localMemoryUserAssemblyAddress);
                logger.LogInformation("memory end at {addr:X8}", localMemoryUserAssemblyAddress + userAssembly.modBaseSize);
            }

            nuint rip = localMemoryUserAssemblyAddress + (uint)offset;
            rip += *(uint*)(rip + 1) + 5;
            rip += *(uint*)(rip + 3) + 7;

            nuint ptr = 0;
            nuint address = rip - localMemoryUserAssemblyAddress + (nuint)userAssembly.modBaseAddr;
            logger.LogInformation("UnsafeReadModuleMemory at {addr:x8}|{uaAddr:x8}", address, (nuint)userAssembly.modBaseAddr);
            while (ptr == 0)
            {
                // Critial: The pointer here is always returning 0
                // Make this a dead loop.
                if (UnsafeReadProcessMemory(gameProcess, address, out ptr))
                {
                    logger.LogInformation("UnsafeReadProcessMemory succeed {addr:x8}", ptr);
                }
                else
                {
                    logger.LogInformation("UnsafeReadProcessMemory failed");
                }

                Thread.Sleep(100);
            }

            logger.LogInformation("ptr {addr}", ptr);
            rip = ptr - (nuint)unityPlayer.modBaseAddr + localMemoryUnityPlayerAddress;

            while (*(byte*)rip == 0xE8 || *(byte*)rip == 0xE9)
            {
                rip += *(uint*)(rip + 1) + 5;
            }

            nuint localMemoryActualAddress = rip + *(uint*)(rip + 2) + 6;
            nuint actualOffset = localMemoryActualAddress - localMemoryUnityPlayerAddress;
            fpsAddress = (nuint)(unityPlayer.modBaseAddr + actualOffset);
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