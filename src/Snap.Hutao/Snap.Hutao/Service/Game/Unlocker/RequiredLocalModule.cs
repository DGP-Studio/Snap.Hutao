// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.ProcessStatus;
using System.Diagnostics;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Service.Game.Unlocker;

internal readonly struct RequiredLocalModule : IDisposable
{
    public readonly bool HasValue = false;
    public readonly Module UnityPlayer;
    public readonly Module UserAssembly;

    public RequiredLocalModule(in HMODULE unityPlayer, in HMODULE userAssembly)
    {
        HasValue = true;
        HANDLE process = Process.GetCurrentProcess().Handle;

        K32GetModuleInformation(process, unityPlayer, out MODULEINFO upInfo);
        UnityPlayer = new((nuint)(nint)unityPlayer, upInfo.SizeOfImage);

        K32GetModuleInformation(process, userAssembly, out MODULEINFO uaInfo);
        UserAssembly = new((nuint)(nint)userAssembly, uaInfo.SizeOfImage);
    }

    public void Dispose()
    {
        FreeLibrary((nint)UnityPlayer.Address);
        FreeLibrary((nint)UserAssembly.Address);
    }
}