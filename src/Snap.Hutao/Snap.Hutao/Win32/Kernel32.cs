// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Security;
using Snap.Hutao.Win32.System.Console;
using Snap.Hutao.Win32.System.LibraryLoader;
using Snap.Hutao.Win32.System.ProcessStatus;
using Snap.Hutao.Win32.System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SH002")]
[SuppressMessage("", "SYSLIB1054")]
internal static class Kernel32
{
    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern BOOL AllocConsole();

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL CloseHandle(Foundation.HANDLE hObject);

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static unsafe extern Foundation.HANDLE CreateEventW([AllowNull] SECURITY_ATTRIBUTES* lpEventAttributes, BOOL bManualReset, BOOL bInitialState, [AllowNull] PCWSTR lpName);

    [DebuggerStepThrough]
    public static unsafe Foundation.HANDLE CreateEventW(ref readonly SECURITY_ATTRIBUTES eventAttributes, BOOL bManualReset, BOOL bInitialState, [AllowNull] ReadOnlySpan<char> name)
    {
        fixed (SECURITY_ATTRIBUTES* lpEventAttributes = &eventAttributes)
        {
            fixed (char* lpName = name)
            {
                return CreateEventW(lpEventAttributes, bManualReset, bInitialState, lpName);
            }
        }
    }

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern BOOL FreeConsole();

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern BOOL FreeLibrary(HMODULE hLibModule);

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static unsafe extern BOOL GetConsoleMode(Foundation.HANDLE hConsoleHandle, CONSOLE_MODE* lpMode);

    [SuppressMessage("", "SH002")]
    [DebuggerStepThrough]
    public static unsafe BOOL GetConsoleMode(Foundation.HANDLE hConsoleHandle, out CONSOLE_MODE mode)
    {
        fixed (CONSOLE_MODE* lpMode = &mode)
        {
            return GetConsoleMode(hConsoleHandle, lpMode);
        }
    }

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern WIN32_ERROR GetLastError();

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern Foundation.HANDLE GetStdHandle(STD_HANDLE nStdHandle);

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static unsafe extern BOOL K32EnumProcessModules(Foundation.HANDLE hProcess, HMODULE* lphModule, uint cb, uint* lpcbNeeded);

    [DebuggerStepThrough]
    public static unsafe BOOL K32EnumProcessModules(Foundation.HANDLE hProcess, Span<HMODULE> hModules, out uint cbNeeded)
    {
        fixed (HMODULE* lphModule = hModules)
        {
            fixed (uint* lpcbNeeded = &cbNeeded)
            {
                return K32EnumProcessModules(hProcess, lphModule, (uint)(hModules.Length * sizeof(HMODULE)), lpcbNeeded);
            }
        }
    }

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern uint K32GetModuleBaseNameW(Foundation.HANDLE hProcess, [AllowNull] HMODULE hModule, PWSTR lpBaseName, uint nSize);

    [DebuggerStepThrough]
    public static unsafe uint K32GetModuleBaseNameW(Foundation.HANDLE hProcess, [AllowNull] HMODULE hModule, Span<char> baseName)
    {
        fixed (char* lpBaseName = baseName)
        {
            return K32GetModuleBaseNameW(hProcess, hModule, lpBaseName, (uint)baseName.Length);
        }
    }

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static unsafe extern BOOL K32GetModuleInformation(Foundation.HANDLE hProcess, HMODULE hModule, MODULEINFO* lpmodinfo, uint cb);

    [DebuggerStepThrough]
    public static unsafe BOOL K32GetModuleInformation(Foundation.HANDLE hProcess, HMODULE hModule, out MODULEINFO modinfo)
    {
        fixed (MODULEINFO* lpmodinfo = &modinfo)
        {
            return K32GetModuleInformation(hProcess, hModule, lpmodinfo, (uint)sizeof(MODULEINFO));
        }
    }

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern HMODULE LoadLibraryExW(PCWSTR lpLibFileName, [AllowNull] Foundation.HANDLE hFile, LOAD_LIBRARY_FLAGS dwFlags);

    [DebuggerStepThrough]
    public static unsafe HMODULE LoadLibraryExW(ReadOnlySpan<char> libFileName, [AllowNull] Foundation.HANDLE hFile, LOAD_LIBRARY_FLAGS dwFlags)
    {
        fixed (char* lpLibFileName = libFileName)
        {
            return LoadLibraryExW(lpLibFileName, hFile, dwFlags);
        }
    }

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern Foundation.HANDLE OpenProcess(PROCESS_ACCESS_RIGHTS dwDesiredAccess, BOOL bInheritHandle, uint dwProcessId);

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static unsafe extern BOOL ReadProcessMemory(Foundation.HANDLE hProcess, void* lpBaseAddress, void* lpBuffer, nuint nSize, [MaybeNull] nuint* lpNumberOfBytesRead);

    [DebuggerStepThrough]
    public static unsafe BOOL ReadProcessMemory(Foundation.HANDLE hProcess, void* lpBaseAddress, Span<byte> buffer, [MaybeNull] out nuint numberOfBytesRead)
    {
        fixed (byte* lpBuffer = buffer)
        {
            fixed (nuint* lpNumberOfBytesRead = &numberOfBytesRead)
            {
                return ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, (uint)buffer.Length, lpNumberOfBytesRead);
            }
        }
    }

    [DebuggerStepThrough]
    public static unsafe BOOL ReadProcessMemory<T>(Foundation.HANDLE hProcess, void* lpBaseAddress, ref T buffer, [MaybeNull] out nuint numberOfBytesRead)
        where T : unmanaged
    {
        fixed (T* lpBuffer = &buffer)
        {
            fixed (nuint* lpNumberOfBytesRead = &numberOfBytesRead)
            {
                return ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, (uint)sizeof(T), lpNumberOfBytesRead);
            }
        }
    }

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern BOOL SetConsoleMode(Foundation.HANDLE hConsoleHandle, CONSOLE_MODE dwMode);

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern BOOL SetConsoleTitleW(PCWSTR lpConsoleTitle);

    [DebuggerStepThrough]
    public static unsafe BOOL SetConsoleTitleW(ReadOnlySpan<char> consoleTitle)
    {
        fixed (char* lpConsoleTitle = consoleTitle)
        {
            return SetConsoleTitleW(lpConsoleTitle);
        }
    }

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern BOOL SetEvent(Foundation.HANDLE hEvent);

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static unsafe extern BOOL WriteProcessMemory(Foundation.HANDLE hProcess, void* lpBaseAddress, void* lpBuffer, nuint nSize, nuint* lpNumberOfBytesWritten);

    [DebuggerStepThrough]
    public static unsafe BOOL WriteProcessMemory(Foundation.HANDLE hProcess, void* lpBaseAddress, ReadOnlySpan<byte> buffer, out nuint numberOfBytesWritten)
    {
        fixed (byte* lpBuffer = buffer)
        {
            fixed (nuint* lpNumberOfBytesWritten = &numberOfBytesWritten)
            {
                return WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, (uint)buffer.Length, lpNumberOfBytesWritten);
            }
        }
    }

    [DebuggerStepThrough]
    public static unsafe BOOL WriteProcessMemory<T>(Foundation.HANDLE hProcess, void* lpBaseAddress, ref readonly T buffer, out nuint numberOfBytesWritten)
        where T : unmanaged
    {
        fixed (T* lpBuffer = &buffer)
        {
            fixed (nuint* lpNumberOfBytesWritten = &numberOfBytesWritten)
            {
                return WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, (uint)sizeof(T), lpNumberOfBytesWritten);
            }
        }
    }
}