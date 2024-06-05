// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.ProcessStatus;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Service.Game.Unlocker;

internal readonly struct RequiredLocalModule : IDisposable
{
    public readonly bool HasValue = false;
    public readonly Module UnityPlayer;
    public readonly Module UserAssembly;

    public RequiredLocalModule(nint unityPlayerAddress, nint userAssemblyAddress)
    {
        HasValue = true;
        HANDLE process = Process.GetCurrentProcess().Handle;

        K32GetModuleInformation(process, unityPlayerAddress, out MODULEINFO upInfo);
        UnityPlayer = new((nuint)unityPlayerAddress, upInfo.SizeOfImage);

        K32GetModuleInformation(process, userAssemblyAddress, out MODULEINFO uaInfo);
        UserAssembly = new((nuint)userAssemblyAddress, uaInfo.SizeOfImage);
    }

    public void Dispose()
    {
        NativeLibrary.Free((nint)UnityPlayer.Address);
        NativeLibrary.Free((nint)UserAssembly.Address);
    }
}