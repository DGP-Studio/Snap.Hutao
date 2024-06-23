// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using System.Diagnostics;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// Credit to https://github.com/34736384/genshin-fps-unlock
/// </summary>
internal static class GameFpsAddress
{
#pragma warning disable SA1310
    private const byte ASM_CALL = 0xE8;
    private const byte ASM_JMP = 0xE9;
#pragma warning restore SA1310

    public static unsafe void UnsafeFindFpsAddress(GameFpsUnlockerContext context, in RequiredRemoteModule remoteModule, in RequiredLocalModule localModule)
    {
        int offsetToUserAssembly = IndexOfPattern(localModule.UserAssembly.AsSpan());
        HutaoException.ThrowIfNot(offsetToUserAssembly >= 0, SH.ServiceGameUnlockerInterestedPatternNotFound);

        nuint rip = localModule.UserAssembly.Address + (uint)offsetToUserAssembly;
        rip += 5U;
        rip += (nuint)(*(int*)(rip + 2U) + 6);

        nuint remoteVirtualAddress = remoteModule.UserAssembly.Address + (rip - localModule.UserAssembly.Address);

        nuint ptr = 0;
        SpinWait.SpinUntil(() => UnsafeReadProcessMemory(context.AllAccess, remoteVirtualAddress, out ptr) && ptr != 0);

        nuint localVirtualAddress = ptr - remoteModule.UnityPlayer.Address + localModule.UnityPlayer.Address;

        while (*(byte*)localVirtualAddress is ASM_CALL or ASM_JMP)
        {
            localVirtualAddress += (nuint)(*(int*)(localVirtualAddress + 1) + 5);
        }

        localVirtualAddress += *(uint*)(localVirtualAddress + 2) + 6;
        nuint relativeVirtualAddress = localVirtualAddress - localModule.UnityPlayer.Address;
        context.FpsAddress = remoteModule.UnityPlayer.Address + relativeVirtualAddress;
    }

    private static int IndexOfPattern(in ReadOnlySpan<byte> memory)
    {
        // B9 3C 00 00 00 FF 15
        ReadOnlySpan<byte> part = [0xB9, 0x3C, 0x00, 0x00, 0x00, 0xFF, 0x15];
        return memory.IndexOf(part);
    }

    [SuppressMessage("", "SH002")]
    private static unsafe bool UnsafeReadProcessMemory(HANDLE hProcess, nuint baseAddress, out nuint value)
    {
        value = 0;
        bool result = ReadProcessMemory(hProcess, (void*)baseAddress, ref value, out _);
        HutaoException.ThrowIfNot(result, SH.ServiceGameUnlockerReadProcessMemoryPointerAddressFailed);
        return result;
    }
}