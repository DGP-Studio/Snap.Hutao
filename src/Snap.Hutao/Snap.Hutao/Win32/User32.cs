// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Accessibility;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
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
    public static extern unsafe HWND CreateWindowExW(WINDOW_EX_STYLE dwExStyle, [Optional] PCWSTR lpClassName, [Optional] PCWSTR lpWindowName, WINDOW_STYLE dwStyle, int X, int Y, int nWidth, int nHeight, [Optional] HWND hWndParent, [Optional] HMENU hMenu, [Optional] HINSTANCE hInstance, [Optional] void* lpParam);

    [DebuggerStepThrough]
    public static unsafe HWND CreateWindowExW(WINDOW_EX_STYLE dwExStyle, [Optional] string className, [Optional] string windowName, WINDOW_STYLE dwStyle, int X, int Y, int nWidth, int nHeight, [Optional] HWND hWndParent, [Optional] HMENU hMenu, [Optional] HINSTANCE hInstance, [Optional] void* lpParam)
    {
        fixed (char* lpClassName = className)
        {
            fixed (char* lpWindowName = windowName)
            {
                return CreateWindowExW(dwExStyle, lpClassName, lpWindowName, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
            }
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern LRESULT DefWindowProcW(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL DestroyWindow(HWND hWnd);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL EnableWindow([In] HWND hWnd, [In] BOOL bEnable);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern HWND GetForegroundWindow();

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe uint GetWindowThreadProcessId(HWND hWnd, [MaybeNull] uint* lpdwProcessId);

    [DebuggerStepThrough]
    public static unsafe uint GetWindowThreadProcessId(HWND hWnd, out uint dwProcessId)
    {
        fixed (uint* lpdwProcessId = &dwProcessId)
        {
            return GetWindowThreadProcessId(hWnd, lpdwProcessId);
        }
    }

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
    public static extern unsafe ushort RegisterClassW(WNDCLASSW* lpWndClass);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows6.0.6000")]
    public static extern BOOL RegisterHotKey([Optional] HWND hWnd, int id, HOT_KEY_MODIFIERS fsModifiers, uint vk);

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
    public static extern unsafe uint SendInput(uint cInputs, INPUT* pInputs, int cbSize);

    [DebuggerStepThrough]
    public static unsafe uint SendInput(ReadOnlySpan<INPUT> inputs, int cbSize)
    {
        fixed (INPUT* pInputs = inputs)
        {
            return SendInput((uint)inputs.Length, pInputs, cbSize);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL SetForegroundWindow(HWND hWnd);

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

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern HWINEVENTHOOK SetWinEventHook(uint eventMin, uint eventMax, HMODULE hmodWinEventProc, WINEVENTPROC pfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL ShowWindow(HWND hWnd, SHOW_WINDOW_CMD nCmdShow);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL UnhookWinEvent(HWINEVENTHOOK hWinEventHook);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL UnregisterHotKey([Optional] HWND hWnd, int id);
}