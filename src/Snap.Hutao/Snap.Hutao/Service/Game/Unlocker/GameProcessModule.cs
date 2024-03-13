// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Memory;
using Snap.Hutao.Win32.System.ProcessStatus;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Service.Game.Unlocker;

internal static class GameProcessModule
{
    public static async ValueTask<ValueResult<FindModuleResult, RequiredGameModule>> FindModuleAsync(GameFpsUnlockerState state)
    {
        ValueStopwatch watch = ValueStopwatch.StartNew();
        using (PeriodicTimer timer = new(state.TimingOptions.FindModuleDelay))
        {
            while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                FindModuleResult result = UnsafeGetGameModuleInfo((HANDLE)state.GameProcess.Handle, out RequiredGameModule gameModule);
                if (result == FindModuleResult.Ok)
                {
                    return new(FindModuleResult.Ok, gameModule);
                }

                if (result == FindModuleResult.NoModuleFound)
                {
                    return new(FindModuleResult.NoModuleFound, default);
                }

                if (watch.GetElapsedTime() > state.TimingOptions.FindModuleLimit)
                {
                    break;
                }
            }
        }

        return new(FindModuleResult.TimeLimitExeeded, default);
    }

    private static FindModuleResult UnsafeGetGameModuleInfo(in HANDLE hProcess, out RequiredGameModule info)
    {
        FindModuleResult unityPlayerResult = UnsafeFindModule(hProcess, "UnityPlayer.dll", out Module unityPlayer);
        FindModuleResult userAssemblyResult = UnsafeFindModule(hProcess, "UserAssembly.dll", out Module userAssembly);

        if (unityPlayerResult is FindModuleResult.Ok && userAssemblyResult is FindModuleResult.Ok)
        {
            info = new(unityPlayer, userAssembly);
            return FindModuleResult.Ok;
        }

        if (unityPlayerResult is FindModuleResult.NoModuleFound && userAssemblyResult is FindModuleResult.NoModuleFound)
        {
            info = default;
            return FindModuleResult.NoModuleFound;
        }

        info = default;
        return FindModuleResult.ModuleNotLoaded;
    }

    private static unsafe FindModuleResult UnsafeFindModule(in HANDLE hProcess, in ReadOnlySpan<char> moduleName, out Module module)
    {
        HMODULE[] buffer = new HMODULE[128];
        if (!K32EnumProcessModules(hProcess, buffer, out uint actualSize))
        {
            Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
        }

        if (actualSize == 0)
        {
            module = default!;
            return FindModuleResult.NoModuleFound;
        }

        foreach (ref readonly HMODULE hModule in buffer.AsSpan()[..(int)(actualSize / sizeof(HMODULE))])
        {
            char[] baseName = new char[256];

            if (K32GetModuleBaseNameW(hProcess, hModule, baseName) == 0)
            {
                continue;
            }

            fixed (char* lpBaseName = baseName)
            {
                if (!moduleName.SequenceEqual(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(lpBaseName)))
                {
                    continue;
                }
            }

            if (!K32GetModuleInformation(hProcess, hModule, out MODULEINFO moduleInfo))
            {
                continue;
            }

            module = new((nuint)moduleInfo.lpBaseOfDll, moduleInfo.SizeOfImage);
            return FindModuleResult.Ok;
        }

        module = default;
        return FindModuleResult.ModuleNotLoaded;
    }
}