// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
    /// <param name="gameProcess">游戏进程</param>
    /// <param name="targetFPS">目标fps</param>
    public GameFpsUnlocker(Process gameProcess, int targetFPS)
    {
        Must.Range(targetFPS >= 30 && targetFPS <= 2000, "Target FPS threshold exceeded");

        TargetFps = targetFPS;
        this.gameProcess = gameProcess;
    }

    /// <inheritdoc/>
    public int TargetFps { get; set; }

    /// <inheritdoc/>
    public async Task UnlockAsync(TimeSpan findModuleDelay, TimeSpan findModuleLimit, TimeSpan adjustFpsDelay)
    {
        Verify.Operation(isValid, "This Unlocker is invalid");

        MODULEENTRY32 unityPlayer = await FindModuleAsync(findModuleDelay, findModuleLimit).ConfigureAwait(false);

        // Read UnityPlayer.dll
        UnsafeTryReadModuleMemoryFindFpsAddress(unityPlayer);

        // When player switch between scenes, we have to re adjust the fps
        // So we keep a loop here
        await LoopAdjustFpsAsync(adjustFpsDelay).ConfigureAwait(false);
    }

    private static unsafe bool UnsafeReadModuleMemory(Process process, MODULEENTRY32 entry, out Span<byte> memory)
    {
        memory = new byte[entry.modBaseSize];

        fixed (byte* lpBuffer = memory)
        {
            return ReadProcessMemory((HANDLE)process.Handle, entry.modBaseAddr, lpBuffer, entry.modBaseSize, null);
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

    private async Task<MODULEENTRY32> FindModuleAsync(TimeSpan findModuleDelay, TimeSpan findModuleLimit)
    {
        ValueStopwatch watch = ValueStopwatch.StartNew();

        while (true)
        {
            MODULEENTRY32 module = UnsafeFindModule(gameProcess.Id, "UnityPlayer.dll"u8);
            if (!StructMarshal.IsDefault(module))
            {
                return module;
            }

            if (watch.GetElapsedTime() > findModuleLimit)
            {
                break;
            }

            await Task.Delay(findModuleDelay).ConfigureAwait(false);
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
                    UnsafeWriteModuleMemory(gameProcess, fpsAddress, TargetFps);
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

    private unsafe void UnsafeTryReadModuleMemoryFindFpsAddress(MODULEENTRY32 unityPlayer)
    {
        bool readOk = UnsafeReadModuleMemory(gameProcess, unityPlayer, out Span<byte> memory);
        Verify.Operation(readOk, "读取内存失败");

        // Find FPS offset
        // 7F 0F              jg   0x11
        // 8B 05 ? ? ? ?      mov eax, dword ptr[rip+?]
        int adr = memory.IndexOf(new byte[] { 0x7F, 0x0F, 0x8B, 0x05 });

        Must.Range(adr >= 0, $"未匹配到FPS字节");

        fixed (byte* pSpan = memory)
        {
            int rip = adr + 2;
            int rel = *(int*)(pSpan + rip + 2); // Unsafe.ReadUnaligned<int>(ref image[rip + 2]);
            int ofs = rip + rel + 6;
            fpsAddress = (nuint)(unityPlayer.modBaseAddr + ofs);
        }
    }
}