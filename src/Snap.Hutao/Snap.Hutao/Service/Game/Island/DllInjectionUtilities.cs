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

    [DllImport(HutaoNativeMethods.DllName, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern HRESULT DllInjectionUtilitiesInjectUsingWindowsHook(PCWSTR dllPath, PCWSTR functionName, int processId);
}
