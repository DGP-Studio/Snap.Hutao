// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Memory;
using Snap.Hutao.Win32.System.ProcessStatus;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Service.Game.Unlocker;

internal static class GameProcessModule
{
    public static async ValueTask<ValueResult<FindModuleResult, RequiredRemoteModule>> FindModuleAsync(GameFpsUnlockerContext state)
    {
        ValueStopwatch watch = ValueStopwatch.StartNew();
        using (PeriodicTimer timer = new(state.Options.FindModuleDelay))
        {
            while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                FindModuleResult result = UnsafeGetGameModuleInfo(state.AllAccess, out RequiredRemoteModule gameModule);
                if (result == FindModuleResult.Ok)
                {
                    return new(FindModuleResult.Ok, gameModule);
                }

                if (result == FindModuleResult.NoModuleFound)
                {
                    return new(FindModuleResult.NoModuleFound, default);
                }

                if (watch.GetElapsedTime() > state.Options.FindModuleLimit)
                {
                    break;
                }
            }
        }

        return new(FindModuleResult.TimeLimitExeeded, default);
    }

    private static FindModuleResult UnsafeGetGameModuleInfo(in HANDLE hProcess, out RequiredRemoteModule info)
    {
        FindModuleResult result = UnsafeFindModule(hProcess, GameConstants.YuanShenFileName, GameConstants.GenshinImpactFileName, out Module executable);

        if (result is FindModuleResult.Ok)
        {
            info = new(executable);
            return FindModuleResult.Ok;
        }

        if (result is FindModuleResult.NoModuleFound)
        {
            info = default;
            return FindModuleResult.NoModuleFound;
        }

        info = default;
        return FindModuleResult.ModuleNotLoaded;
    }

    private static unsafe FindModuleResult UnsafeFindModule(in HANDLE hProcess, in ReadOnlySpan<char> moduleName1, in ReadOnlySpan<char> moduleName2, out Module module)
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
                ReadOnlySpan<char> baseNameSpan = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(lpBaseName);
                if (!moduleName1.SequenceEqual(baseNameSpan) || !moduleName2.SequenceEqual(baseNameSpan))
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