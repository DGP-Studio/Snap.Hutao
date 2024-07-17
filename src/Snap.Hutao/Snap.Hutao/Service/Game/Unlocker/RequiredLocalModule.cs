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
    public readonly Module Executable;

    private readonly HMODULE hModuleExecutable;

    public RequiredLocalModule(HMODULE executable)
    {
        hModuleExecutable = executable;

        // Align the pointer
        nint executableMappedView = (nint)(executable & ~0x3L);

        Executable = new((nuint)executableMappedView, GetImageSize(executableMappedView));
        HasValue = true;
    }

    public void Dispose()
    {
        FreeLibrary(hModuleExecutable);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe uint GetImageSize(nint baseAddress)
    {
        IMAGE_DOS_HEADER* pImageDosHeader = (IMAGE_DOS_HEADER*)baseAddress;
        IMAGE_NT_HEADERS64* pImageNtHeader = (IMAGE_NT_HEADERS64*)(pImageDosHeader->e_lfanew + baseAddress);
        return pImageNtHeader->OptionalHeader.SizeOfImage;
    }
}