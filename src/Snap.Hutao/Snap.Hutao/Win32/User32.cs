// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class User32
{
    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe uint GetWindowThreadProcessId(HWND hWnd, [MaybeNull] uint* lpdwProcessId);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern MESSAGEBOX_RESULT MessageBoxExW([Optional] HWND hWnd, [Optional] PCWSTR lpText, [Optional] PCWSTR lpCaption, MESSAGEBOX_STYLE uType, ushort wLanguageId);

    [DebuggerStepThrough]
    public static unsafe MESSAGEBOX_RESULT MessageBoxExW([Optional] HWND hWnd, ReadOnlySpan<char> text, [Optional] ReadOnlySpan<char> caption, MESSAGEBOX_STYLE uType, ushort wLanguageId)
    {
        fixed (char* lpText = text)
        {
            fixed (char* lpCaption = caption)
            {
                return MessageBoxExW(hWnd, lpText, lpCaption, uType, wLanguageId);
            }
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL PostThreadMessageW(uint idThread, uint Msg, WPARAM wParam, LPARAM lParam);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern HANDLE RemovePropW(HWND hWnd, PCWSTR lpString);

    [DebuggerStepThrough]
    public static unsafe HANDLE RemovePropW(HWND hWnd, string @string)
    {
        fixed (char* lpString = @string)
        {
            return RemovePropW(hWnd, lpString);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL SetPropW(HWND hWnd, PCWSTR lpString, [Optional] HANDLE hData);

    [DebuggerStepThrough]
    public static unsafe BOOL SetPropW(HWND hWnd, string @string, [Optional] HANDLE hData)
    {
        fixed (char* lpString = @string)
        {
            return SetPropW(hWnd, lpString, hData);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern nint SetWindowLongPtrW(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint dwNewLong);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern HHOOK SetWindowsHookExW(WINDOWS_HOOK_ID idHook, HOOKPROC lpfn, [Optional] HINSTANCE hmod, uint dwThreadId);
}