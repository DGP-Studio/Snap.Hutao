// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Gdi;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SH002")]
[SuppressMessage("", "SYSLIB1054")]
internal static class User32
{
    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern BOOL AttachThreadInput(uint idAttach, uint idAttachTo, BOOL fAttach);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern HWND FindWindowExW([AllowNull] HWND hWndParent, [AllowNull] HWND hWndChildAfter, [AllowNull] PCWSTR lpszClass, [AllowNull] PCWSTR lpszWindow);

    [DebuggerStepThrough]
    public static unsafe HWND FindWindowExW([AllowNull] HWND hWndParent, [AllowNull] HWND hWndChildAfter, [AllowNull] string szClass, [AllowNull] string szWindow)
    {
        fixed (char* lpszClass = szClass)
        {
            fixed (char* lpszWindow = szWindow)
            {
                return FindWindowExW(hWndParent, hWndChildAfter, lpszClass, lpszWindow);
            }
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern HDC GetDC([AllowNull] HWND hWnd);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows10.0.14393")]
    public static extern uint GetDpiForWindow(HWND hwnd);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern HWND GetForegroundWindow();

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern nint GetWindowLongPtrW(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static unsafe extern BOOL GetWindowPlacement(HWND hWnd, WINDOWPLACEMENT* lpwndpl);

    [DebuggerStepThrough]
    public static unsafe BOOL GetWindowPlacement(HWND hWnd, ref WINDOWPLACEMENT wndpl)
    {
        fixed (WINDOWPLACEMENT* lpwndpl = &wndpl)
        {
            return GetWindowPlacement(hWnd, lpwndpl);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static unsafe extern uint GetWindowThreadProcessId(HWND hWnd, [MaybeNull] uint* lpdwProcessId);

    [DebuggerStepThrough]
    public static unsafe uint GetWindowThreadProcessId(HWND hWnd, [MaybeNull] out uint dwProcessId)
    {
        fixed (uint* lpdwProcessId = &dwProcessId)
        {
            return GetWindowThreadProcessId(hWnd, lpdwProcessId);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
    [SupportedOSPlatform("windows5.0")]
    public static unsafe extern ushort RegisterClassW(WNDCLASSW* lpWndClass);

    public static unsafe ushort RegisterClassW(ref readonly WNDCLASSW lpWndClass)
    {
        fixed (WNDCLASSW* pWndClass = &lpWndClass)
        {
            return RegisterClassW(pWndClass);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows6.0.6000")]
    public static extern BOOL RegisterHotKey([AllowNull] HWND hWnd, int id, HOT_KEY_MODIFIERS fsModifiers, uint vk);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern int ReleaseDC([AllowNull] HWND hWnd, HDC hDC);

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

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true, SetLastError = true)]
    [SupportedOSPlatform("windows5.0")]
    public static unsafe extern uint SendInput(uint cInputs, INPUT* pInputs, int cbSize);

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

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL SetPropW(HWND hWnd, PCWSTR lpString, [AllowNull] HANDLE hData);

    [DebuggerStepThrough]
    public static unsafe BOOL SetPropW(HWND hWnd, string @string, [AllowNull] HANDLE hData)
    {
        fixed (char* lpString = @string)
        {
            return SetPropW(hWnd, lpString, hData);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern nint SetWindowLongPtrW(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint dwNewLong);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL UnregisterHotKey([AllowNull] HWND hWnd, int id);
}