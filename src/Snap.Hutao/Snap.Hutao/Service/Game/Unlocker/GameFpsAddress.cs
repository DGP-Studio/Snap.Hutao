// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
        Span<byte> executableSpan = localModule.Executable.AsSpan();
        int offsetToExecutable = 0;
        nuint localVirtualAddress = 0;
        do
        {
            int index = IndexOfPattern(executableSpan[offsetToExecutable..]);
            if (index < 0)
            {
                break;
            }

            offsetToExecutable += index;

            nuint rip = localModule.Executable.Address + (uint)offsetToExecutable;
            rip += 5U;
            rip += (nuint)(*(int*)(rip + 1U) + 5);

            if (*(byte*)rip is ASM_JMP)
            {
                localVirtualAddress = rip;
                break;
            }
        }
        while (true);

        ArgumentOutOfRangeException.ThrowIfZero(localVirtualAddress);

        while (*(byte*)localVirtualAddress is ASM_CALL or ASM_JMP)
        {
            localVirtualAddress += (nuint)(*(int*)(localVirtualAddress + 1) + 5);
        }

        localVirtualAddress += *(uint*)(localVirtualAddress + 2) + 6;
        nuint relativeVirtualAddress = localVirtualAddress - localModule.Executable.Address;
        context.FpsAddress = remoteModule.Executable.Address + relativeVirtualAddress;
    }

    private static int IndexOfPattern(in ReadOnlySpan<byte> span)
    {
        // B9 3C 00 00 00 E8
        ReadOnlySpan<byte> part = [0xB9, 0x3C, 0x00, 0x00, 0x00, 0xE8];
        return span.IndexOf(part);
    }
}