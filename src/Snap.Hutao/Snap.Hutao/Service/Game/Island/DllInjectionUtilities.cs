// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Game.Island;

internal static class DllInjectionUtilities
{
    public static unsafe void InjectUsingWindowsHook(ReadOnlySpan<char> dllPath, ReadOnlySpan<char> functionName, int processId)
    {
        fixed (char* pDllPath = dllPath)
        {
            fixed (char* pFunctionName = functionName)
            {
                Marshal.ThrowExceptionForHR(DllInjectionUtilitiesInjectUsingWindowsHook(pDllPath, pFunctionName, processId));
            }
        }
    }

    public static unsafe void InjectUsingWindowsHook2(ReadOnlySpan<char> dllPath, ReadOnlySpan<char> functionName, int processId)
    {
        fixed (char* pDllPath = dllPath)
        {
            fixed (char* pFunctionName = functionName)
            {
                Marshal.ThrowExceptionForHR(DllInjectionUtilitiesInjectUsingWindowsHook2(pDllPath, pFunctionName, processId));
            }
        }
    }

    public static unsafe void InjectUsingRemoteThread(ReadOnlySpan<char> dllPath, ReadOnlySpan<char> functionName, int processId)
    {
        fixed (char* pDllPath = dllPath)
        {
            fixed (char* pFunctionName = functionName)
            {
                Marshal.ThrowExceptionForHR(DllInjectionUtilitiesInjectUsingRemoteThread(pDllPath, pFunctionName, processId));
            }
        }
    }

    // Security note: These method uses LOAD_WITH_ALTERED_SEARCH_PATH to load the DLL.
    // Which can cause arbitrary DLL in the same directory with the same name that required by the DLL to be loaded.
    // We should pre-check the directory to ensure that it only contains the expected DLL.
    [DllImport(HutaoNativeMethods.DllName, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern HRESULT DllInjectionUtilitiesInjectUsingWindowsHook(PCWSTR dllPath, PCWSTR functionName, int processId);

    [DllImport(HutaoNativeMethods.DllName, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern HRESULT DllInjectionUtilitiesInjectUsingWindowsHook2(PCWSTR dllPath, PCWSTR functionName, int processId);

    [DllImport(HutaoNativeMethods.DllName, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern HRESULT DllInjectionUtilitiesInjectUsingRemoteThread(PCWSTR dllPath, PCWSTR functionName, int processId);
}