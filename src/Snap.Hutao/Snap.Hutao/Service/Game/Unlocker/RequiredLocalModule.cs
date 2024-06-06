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

    private readonly HMODULE hModuleUnityPlayer;
    private readonly HMODULE hModuleUserAssembly;

    [SuppressMessage("", "SH002")]
    public RequiredLocalModule(HMODULE unityPlayer, HMODULE userAssembly)
    {
        hModuleUnityPlayer = unityPlayer;
        hModuleUserAssembly = userAssembly;

        // Align the pointer
        nint unityPlayerMappedView = (nint)(unityPlayer & ~0x3L);
        nint userAssemblyMappedView = (nint)(userAssembly & ~0x3L);

        HasValue = true;
        UnityPlayer = new((nuint)unityPlayerMappedView, GetImageSize(unityPlayerMappedView));
        UserAssembly = new((nuint)userAssemblyMappedView, GetImageSize(userAssemblyMappedView));
    }

    public void Dispose()
    {
        FreeLibrary(hModuleUnityPlayer);
        FreeLibrary(hModuleUserAssembly);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe uint GetImageSize(nint baseAddress)
    {
        IMAGE_DOS_HEADER* pImageDosHeader = (IMAGE_DOS_HEADER*)baseAddress;
        IMAGE_NT_HEADERS64* pImageNtHeader = (IMAGE_NT_HEADERS64*)(pImageDosHeader->e_lfanew + baseAddress);
        return pImageNtHeader->OptionalHeader.SizeOfImage;
    }
}