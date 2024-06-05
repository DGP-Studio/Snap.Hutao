// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Diagnostics.Debug;
using Snap.Hutao.Win32.System.SystemService;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Service.Game.Unlocker;

internal readonly struct RequiredLocalModule : IDisposable
{
    public readonly bool HasValue = false;
    public readonly Module UnityPlayer;
    public readonly Module UserAssembly;

    public unsafe RequiredLocalModule(in HMODULE unityPlayer, in HMODULE userAssembly)
    {
        HasValue = true;
        UnityPlayer = new((nuint)(nint)unityPlayer, ((IMAGE_NT_HEADERS64*)((IMAGE_DOS_HEADER*)(nint)unityPlayer)->e_lfanew)->OptionalHeader.SizeOfImage);
        UserAssembly = new((nuint)(nint)userAssembly, ((IMAGE_NT_HEADERS64*)((IMAGE_DOS_HEADER*)(nint)userAssembly)->e_lfanew)->OptionalHeader.SizeOfImage);
    }

    public void Dispose()
    {
        FreeLibrary((nint)UnityPlayer.Address);
        FreeLibrary((nint)UserAssembly.Address);
    }
}