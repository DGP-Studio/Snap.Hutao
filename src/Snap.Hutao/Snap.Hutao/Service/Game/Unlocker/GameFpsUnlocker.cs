// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Win32;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Diagnostics.ToolHelp;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 游戏帧率解锁器
/// </summary>
internal class GameFpsUnlocker : IGameFpsUnlocker
{
    private readonly Process gameProcess;

    private nuint fpsAddress;
    private bool isValid = true;

    /// <summary>
    /// 构造一个新的 <see cref="Unlocker"/> 对象，
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
        Must.Range(targetFPS >= 30 && targetFPS <= 2000, "目标FPS超过允许值");

        TargetFps = targetFPS;
        this.gameProcess = gameProcess;
    }

    /// <inheritdoc/>
    public int TargetFps { get; set; }

    /// <inheritdoc/>
    public async Task UnlockAsync(TimeSpan findModuleDelay, TimeSpan findModuleLimit, TimeSpan adjustFpsDelay)
    {
        Verify.Operation(isValid, "此解锁器已经失效");

        MODULEENTRY32 unityPlayer = await FindModuleAsync(findModuleDelay, findModuleLimit).ConfigureAwait(false);

        // Read UnityPlayer.dll
        TryReadModuleMemoryFindFpsAddress(unityPlayer);

        // When player switch between scenes, we have to re adjust the fps
        // So we keep a loop here
        await LoopAdjustFpsAsync(adjustFpsDelay).ConfigureAwait(false);
    }

    private static unsafe bool UnsafeReadModuleMemory(Process process, MODULEENTRY32 entry, out Span<byte> image)
    {
        image = new byte[entry.modBaseSize];
        fixed (byte* lpBuffer = image)
        {
            return ReadProcessMemory(process.SafeHandle, entry.modBaseAddr, lpBuffer, entry.modBaseSize, null);
        }
    }

    private static unsafe bool UnsafeWriteModuleMemory(Process process, nuint baseAddress, int write)
    {
        int* lpBuffer = &write;

        return WriteProcessMemory(process.SafeHandle, (void*)baseAddress, lpBuffer, sizeof(int), null);
    }

    private static unsafe MODULEENTRY32 FindModule(int processId, string moduleName)
    {
        using (SafeFileHandle snapshot = CreateToolhelp32Snapshot_SafeHandle(CREATE_TOOLHELP_SNAPSHOT_FLAGS.TH32CS_SNAPMODULE, (uint)processId))
        {
            Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());

            MODULEENTRY32 entry = StructMarshal.MODULEENTRY32();
            bool found = false;

            // First module must be exe. Ignoring it.
            for (Module32First(snapshot, ref entry); Module32Next(snapshot, ref entry);)
            {
                if (entry.th32ProcessID == processId && entry.szModule.AsString() == moduleName)
                {
                    found = true;
                    break;
                }
            }

            return found ? entry : default;
        }
    }

    private async Task<MODULEENTRY32> FindModuleAsync(TimeSpan findModuleDelay, TimeSpan findModuleLimit)
    {
        ValueStopwatch watch = ValueStopwatch.StartNew();

        while (true)
        {
            MODULEENTRY32 module = FindModule(gameProcess.Id, "UnityPlayer.dll");
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
        while (true)
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

            await Task.Delay(adjustFpsDelay).ConfigureAwait(false);
        }
    }

    private unsafe void TryReadModuleMemoryFindFpsAddress(MODULEENTRY32 unityPlayer)
    {
        bool readOk = UnsafeReadModuleMemory(gameProcess, unityPlayer, out Span<byte> image);
        Verify.Operation(readOk, "读取内存失败");

        // Find FPS offset
        // 7F 0F              jg   0x11
        // 8B 05 ? ? ? ?      mov eax, dword ptr[rip+?]
        int adr = image.IndexOf(new byte[] { 0x7F, 0x0F, 0x8B, 0x05 });

        Must.Range(adr >= 0, "未匹配到FPS字节");

        int rip = adr + 2;
        int rel = Unsafe.ReadUnaligned<int>(ref image[rip + 2]);
        int ofs = rip + rel + 6;
        fpsAddress = (nuint)((long)unityPlayer.modBaseAddr + ofs);
    }
}