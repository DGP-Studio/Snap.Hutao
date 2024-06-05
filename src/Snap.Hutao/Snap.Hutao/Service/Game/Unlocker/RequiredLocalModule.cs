// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Diagnostics.Debug;
using Snap.Hutao.Win32.System.SystemService;
using System.Runtime.CompilerServices;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Service.Game.Unlocker;

internal readonly struct RequiredLocalModule : IDisposable
{
    public readonly bool HasValue = false;
    public readonly Module UnityPlayer;
    public readonly Module UserAssembly;

    [SuppressMessage("", "SH002")]
    public RequiredLocalModule(HMODULE unityPlayer, HMODULE userAssembly)
    {
        // Align the pointer
        unityPlayer = (nint)(unityPlayer & ~0x3L);
        userAssembly = (nint)(userAssembly & ~0x3L);

        HasValue = true;
        UnityPlayer = new((nuint)(nint)unityPlayer, GetImageSize(unityPlayer));
        UserAssembly = new((nuint)(nint)userAssembly, GetImageSize(userAssembly));
    }

    public void Dispose()
    {
        FreeLibrary((nint)UnityPlayer.Address);
        FreeLibrary((nint)UserAssembly.Address);
    }

    [SuppressMessage("", "SH002")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe uint GetImageSize(HMODULE hModule)
    {
        return ((IMAGE_NT_HEADERS64*)((IMAGE_DOS_HEADER*)(nint)hModule)->e_lfanew)->OptionalHeader.SizeOfImage;
    }
}