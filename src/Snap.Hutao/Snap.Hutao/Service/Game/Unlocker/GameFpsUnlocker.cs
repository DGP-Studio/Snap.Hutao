// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Memory;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.System.ProcessStatus;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 游戏帧率解锁器
/// Credit to https://github.com/34736384/genshin-fps-unlock
/// </summary>
[HighQuality]
internal sealed class GameFpsUnlocker : IGameFpsUnlocker
{
    private readonly Process gameProcess;
    private readonly LaunchOptions launchOptions;
    private readonly UnlockerStatus status = new();

    /// <summary>
    /// 构造一个新的 <see cref="GameFpsUnlocker"/> 对象，
    /// 每个解锁器只能解锁一次原神的进程，
    /// 再次解锁需要重新创建对象
    /// <para/>
    /// 解锁器需要在管理员模式下才能正确的完成解锁操作，
    /// 非管理员模式不能解锁
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="gameProcess">游戏进程</param>
    public GameFpsUnlocker(IServiceProvider serviceProvider, Process gameProcess)
    {
        launchOptions = serviceProvider.GetRequiredService<LaunchOptions>();
        this.gameProcess = gameProcess;
    }

    /// <inheritdoc/>
    public async ValueTask UnlockAsync(UnlockTimingOptions options, IProgress<UnlockerStatus> progress, CancellationToken token = default)
    {
        Verify.Operation(status.IsUnlockerValid, "This Unlocker is invalid");

        (FindModuleResult result, GameModule moduleEntryInfo) = await FindModuleAsync(options.FindModuleDelay, options.FindModuleLimit).ConfigureAwait(false);
        Verify.Operation(result != FindModuleResult.TimeLimitExeeded, SH.ServiceGameUnlockerFindModuleTimeLimitExeeded);
        Verify.Operation(result != FindModuleResult.NoModuleFound, SH.ServiceGameUnlockerFindModuleNoModuleFound);

        // Read UnityPlayer.dll
        UnsafeFindFpsAddress(moduleEntryInfo);
        progress.Report(status);

        // When player switch between scenes, we have to re adjust the fps
        // So we keep a loop here
        await LoopAdjustFpsAsync(options.AdjustFpsDelay, progress, token).ConfigureAwait(false);
    }

    private static unsafe bool UnsafeReadModulesMemory(Process process, in GameModule moduleEntryInfo, out VirtualMemory memory)
    {
        ref readonly Module unityPlayer = ref moduleEntryInfo.UnityPlayer;
        ref readonly Module userAssembly = ref moduleEntryInfo.UserAssembly;

        memory = new VirtualMemory(unityPlayer.Size + userAssembly.Size);
        byte* lpBuffer = (byte*)memory.Pointer;
        return ReadProcessMemory((HANDLE)process.Handle, (void*)unityPlayer.Address, lpBuffer, unityPlayer.Size)
            && ReadProcessMemory((HANDLE)process.Handle, (void*)userAssembly.Address, lpBuffer + unityPlayer.Size, userAssembly.Size);
    }

    private static unsafe bool UnsafeReadProcessMemory(Process process, nuint baseAddress, out nuint value)
    {
        ulong temp = 0;
        bool result = ReadProcessMemory((HANDLE)process.Handle, (void*)baseAddress, (byte*)&temp, 8);
        Verify.Operation(result, SH.ServiceGameUnlockerReadProcessMemoryPointerAddressFailed);

        value = (nuint)temp;
        return result;
    }

    private static unsafe bool UnsafeWriteProcessMemory(Process process, nuint baseAddress, int value)
    {
        return WriteProcessMemory((HANDLE)process.Handle, (void*)baseAddress, &value, sizeof(int));
    }

    private static unsafe FindModuleResult UnsafeTryFindModule(in HANDLE hProcess, in ReadOnlySpan<char> moduleName, out Module module)
    {
        HMODULE[] buffer = new HMODULE[128];
        uint actualSize;
        fixed (HMODULE* pBuffer = buffer)
        {
            if (!K32EnumProcessModules(hProcess, pBuffer, unchecked((uint)(buffer.Length * sizeof(HMODULE))), out actualSize))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
            }
        }

        if (actualSize == 0)
        {
            module = default!;
            return FindModuleResult.NoModuleFound;
        }

        Span<HMODULE> modules = new(buffer, 0, unchecked((int)(actualSize / sizeof(HMODULE))));

        foreach (ref readonly HMODULE hModule in modules)
        {
            char[] baseName = new char[256];
            fixed (char* lpBaseName = baseName)
            {
                if (K32GetModuleBaseName(hProcess, hModule, lpBaseName, 256) == 0)
                {
                    continue;
                }

                ReadOnlySpan<char> szModuleName = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(lpBaseName);
                if (!szModuleName.SequenceEqual(moduleName))
                {
                    continue;
                }
            }

            if (!K32GetModuleInformation(hProcess, hModule, out MODULEINFO moduleInfo, unchecked((uint)sizeof(MODULEINFO))))
            {
                continue;
            }

            module = new((nuint)moduleInfo.lpBaseOfDll, moduleInfo.SizeOfImage);
            return FindModuleResult.Ok;
        }

        module = default;
        return FindModuleResult.ModuleNotLoaded;
    }

    private static int IndexOfPattern(in ReadOnlySpan<byte> memory)
    {
        // E8 ?? ?? ?? ?? 85 C0 7E 07 E8 ?? ?? ?? ?? EB 05
        int second = 0;
        ReadOnlySpan<byte> secondPart = stackalloc byte[] { 0x85, 0xC0, 0x7E, 0x07, 0xE8, };
        ReadOnlySpan<byte> thirdPart = stackalloc byte[] { 0xEB, 0x05, };

        while (second >= 0 && second < memory.Length)
        {
            second += memory[second..].IndexOf(secondPart);
            if (memory[second - 5].Equals(0xE8) && memory.Slice(second + 9, 2).SequenceEqual(thirdPart))
            {
                return second - 5;
            }

            second += 5;
        }

        return -1;
    }

    private static FindModuleResult UnsafeGetGameModuleInfo(in HANDLE hProcess, out GameModule info)
    {
        FindModuleResult unityPlayerResult = UnsafeTryFindModule(hProcess, "UnityPlayer.dll", out Module unityPlayer);
        FindModuleResult userAssemblyResult = UnsafeTryFindModule(hProcess, "UserAssembly.dll", out Module userAssembly);

        if (unityPlayerResult == FindModuleResult.Ok && userAssemblyResult == FindModuleResult.Ok)
        {
            info = new(unityPlayer, userAssembly);
            return FindModuleResult.Ok;
        }

        if (unityPlayerResult == FindModuleResult.NoModuleFound && userAssemblyResult == FindModuleResult.NoModuleFound)
        {
            info = default;
            return FindModuleResult.NoModuleFound;
        }

        info = default;
        return FindModuleResult.ModuleNotLoaded;
    }

    private async ValueTask<ValueResult<FindModuleResult, GameModule>> FindModuleAsync(TimeSpan findModuleDelay, TimeSpan findModuleLimit)
    {
        ValueStopwatch watch = ValueStopwatch.StartNew();
        using (PeriodicTimer timer = new(findModuleDelay))
        {
            while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                FindModuleResult result = UnsafeGetGameModuleInfo((HANDLE)gameProcess.Handle, out GameModule gameModule);
                if (result == FindModuleResult.Ok)
                {
                    return new(FindModuleResult.Ok, gameModule);
                }

                if (result == FindModuleResult.NoModuleFound)
                {
                    return new(FindModuleResult.NoModuleFound, default);
                }

                if (watch.GetElapsedTime() > findModuleLimit)
                {
                    break;
                }
            }
        }

        return new(FindModuleResult.TimeLimitExeeded, default);
    }

    private async ValueTask LoopAdjustFpsAsync(TimeSpan adjustFpsDelay, IProgress<UnlockerStatus> progress, CancellationToken token)
    {
        using (PeriodicTimer timer = new(adjustFpsDelay))
        {
            while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
            {
                if (!gameProcess.HasExited && status.FpsAddress != 0U)
                {
                    UnsafeWriteProcessMemory(gameProcess, status.FpsAddress, launchOptions.TargetFps);
                    progress.Report(status);
                }
                else
                {
                    status.IsUnlockerValid = false;
                    status.FpsAddress = 0;
                    progress.Report(status);
                    return;
                }
            }
        }
    }

    private unsafe void UnsafeFindFpsAddress(in GameModule moduleEntryInfo)
    {
        bool readOk = UnsafeReadModulesMemory(gameProcess, moduleEntryInfo, out VirtualMemory localMemory);
        Verify.Operation(readOk, SH.ServiceGameUnlockerReadModuleMemoryCopyVirtualMemoryFailed);

        using (localMemory)
        {
            int offset = IndexOfPattern(localMemory.AsSpan()[(int)moduleEntryInfo.UnityPlayer.Size..]);
            Must.Range(offset >= 0, SH.ServiceGameUnlockerInterestedPatternNotFound);

            byte* pLocalMemory = (byte*)localMemory.Pointer;
            ref readonly Module unityPlayer = ref moduleEntryInfo.UnityPlayer;
            ref readonly Module userAssembly = ref moduleEntryInfo.UserAssembly;

            nuint localMemoryUnityPlayerAddress = (nuint)pLocalMemory;
            nuint localMemoryUserAssemblyAddress = localMemoryUnityPlayerAddress + unityPlayer.Size;

            nuint rip = localMemoryUserAssemblyAddress + (uint)offset;
            rip += *(uint*)(rip + 1) + 5;
            rip += *(uint*)(rip + 3) + 7;

            nuint address = userAssembly.Address + (rip - localMemoryUserAssemblyAddress);

            nuint ptr = 0;
            SpinWait.SpinUntil(() => UnsafeReadProcessMemory(gameProcess, address, out ptr) && ptr != 0);

            rip = ptr - unityPlayer.Address + localMemoryUnityPlayerAddress;
            while (*(byte*)rip == 0xE8 || *(byte*)rip == 0xE9)
            {
                rip += (nuint)(*(int*)(rip + 1) + 5);
            }

            nuint localMemoryActualAddress = rip + *(uint*)(rip + 2) + 6;
            nuint actualOffset = localMemoryActualAddress - localMemoryUnityPlayerAddress;
            status.FpsAddress = unityPlayer.Address + actualOffset;
        }
    }

    private readonly struct GameModule
    {
        public readonly bool HasValue = false;
        public readonly Module UnityPlayer;
        public readonly Module UserAssembly;

        public GameModule(in Module unityPlayer, in Module userAssembly)
        {
            HasValue = true;
            UnityPlayer = unityPlayer;
            UserAssembly = userAssembly;
        }
    }

    private readonly struct Module
    {
        public readonly bool HasValue = false;
        public readonly nuint Address;
        public readonly uint Size;

        public Module(nuint address, uint size)
        {
            HasValue = true;
            Address = address;
            Size = size;
        }
    }
}