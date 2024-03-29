// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Memory;
using System.Diagnostics;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Service.Game.Unlocker;

internal static class GameFpsAddress
{
#pragma warning disable SA1310
    private const byte ASM_CALL = 0xE8;
    private const byte ASM_JMP = 0xE9;
#pragma warning restore SA1310

    public static unsafe void UnsafeFindFpsAddress(GameFpsUnlockerContext state, in RequiredGameModule requiredGameModule)
    {
        bool readOk = UnsafeReadModulesMemory(state.GameProcess, requiredGameModule, out VirtualMemory localMemory);
        HutaoException.ThrowIfNot(readOk, HutaoExceptionKind.GameFpsUnlockingFailed, SH.ServiceGameUnlockerReadModuleMemoryCopyVirtualMemoryFailed);

        using (localMemory)
        {
            int offset = IndexOfPattern(localMemory.AsSpan()[(int)requiredGameModule.UnityPlayer.Size..]);
            HutaoException.ThrowIfNot(offset >= 0, HutaoExceptionKind.GameFpsUnlockingFailed, SH.ServiceGameUnlockerInterestedPatternNotFound);

            byte* pLocalMemory = (byte*)localMemory.Pointer;
            ref readonly Module unityPlayer = ref requiredGameModule.UnityPlayer;
            ref readonly Module userAssembly = ref requiredGameModule.UserAssembly;

            nuint localMemoryUnityPlayerAddress = (nuint)pLocalMemory;
            nuint localMemoryUserAssemblyAddress = localMemoryUnityPlayerAddress + unityPlayer.Size;

            nuint rip = localMemoryUserAssemblyAddress + (uint)offset;
            rip += 5U;
            rip += (nuint)(*(int*)(rip + 2U) + 6);

            nuint address = userAssembly.Address + (rip - localMemoryUserAssemblyAddress);

            nuint ptr = 0;
            SpinWait.SpinUntil(() => UnsafeReadProcessMemory(state.GameProcess, address, out ptr) && ptr != 0);

            rip = ptr - unityPlayer.Address + localMemoryUnityPlayerAddress;

            while (*(byte*)rip is ASM_CALL or ASM_JMP)
            {
                rip += (nuint)(*(int*)(rip + 1) + 5);
            }

            nuint localMemoryActualAddress = rip + *(uint*)(rip + 2) + 6;
            nuint actualOffset = localMemoryActualAddress - localMemoryUnityPlayerAddress;
            state.FpsAddress = unityPlayer.Address + actualOffset;
        }
    }

    private static int IndexOfPattern(in ReadOnlySpan<byte> memory)
    {
        // B9 3C 00 00 00 FF 15
        ReadOnlySpan<byte> part = [0xB9, 0x3C, 0x00, 0x00, 0x00, 0xFF, 0x15];
        return memory.IndexOf(part);
    }

    private static unsafe bool UnsafeReadModulesMemory(Process process, in RequiredGameModule moduleEntryInfo, out VirtualMemory memory)
    {
        ref readonly Module unityPlayer = ref moduleEntryInfo.UnityPlayer;
        ref readonly Module userAssembly = ref moduleEntryInfo.UserAssembly;

        memory = new VirtualMemory(unityPlayer.Size + userAssembly.Size);
        return ReadProcessMemory(process.Handle, (void*)unityPlayer.Address, memory.AsSpan()[..(int)unityPlayer.Size], out _)
            && ReadProcessMemory(process.Handle, (void*)userAssembly.Address, memory.AsSpan()[(int)unityPlayer.Size..], out _);
    }

    private static unsafe bool UnsafeReadProcessMemory(Process process, nuint baseAddress, out nuint value)
    {
        value = 0;
        bool result = ReadProcessMemory((HANDLE)process.Handle, (void*)baseAddress, ref value, out _);
        HutaoException.ThrowIfNot(result, HutaoExceptionKind.GameFpsUnlockingFailed, SH.ServiceGameUnlockerReadProcessMemoryPointerAddressFailed);
        return result;
    }
}