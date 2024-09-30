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

[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class User32
{
    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern BOOL AttachThreadInput(uint idAttach, uint idAttachTo, BOOL fAttach);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe BOOL ClientToScreen(HWND hWnd, POINT* lpPoint);

    public static unsafe BOOL ClientToScreen(HWND hWnd, ref POINT point)
    {
        fixed (POINT* lpPoint = &point)
        {
            return ClientToScreen(hWnd, lpPoint);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe HWND CreateWindowExW(WINDOW_EX_STYLE dwExStyle, [Optional] PCWSTR lpClassName, [Optional] PCWSTR lpWindowName, WINDOW_STYLE dwStyle, int X, int Y, int nWidth, int nHeight, [Optional] HWND hWndParent, [Optional] HMENU hMenu, [Optional] HINSTANCE hInstance, [Optional] void* lpParam);

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
    public static extern unsafe BOOL EqualRect(RECT* lprc1, RECT* lprc2);

    [DebuggerStepThrough]
    public static unsafe BOOL EqualRect(ref readonly RECT rc1, ref readonly RECT rc2)
    {
        fixed (RECT* lprc1 = &rc1)
        {
            fixed (RECT* lprc2 = &rc2)
            {
                return EqualRect(lprc1, lprc2);
            }
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern HWND FindWindowExW([Optional] HWND hWndParent, [Optional] HWND hWndChildAfter, [Optional] PCWSTR lpszClass, [Optional] PCWSTR lpszWindow);

    [DebuggerStepThrough]
    public static unsafe HWND FindWindowExW([Optional] HWND hWndParent, [Optional] HWND hWndChildAfter, [Optional] string szClass, [Optional] string szWindow)
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
    public static extern unsafe BOOL GetClientRect(HWND hWnd, RECT* lpRect);

    public static unsafe BOOL GetClientRect(HWND hWnd, out RECT rect)
    {
        fixed (RECT* lpRect = &rect)
        {
            return GetClientRect(hWnd, lpRect);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern HDC GetDC([Optional] HWND hWnd);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows10.0.14393")]
    public static extern uint GetDpiForWindow(HWND hwnd);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern HWND GetForegroundWindow();

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern nint GetWindowLongPtrW(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe BOOL GetWindowPlacement(HWND hWnd, WINDOWPLACEMENT* lpwndpl);

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
    public static extern unsafe BOOL GetWindowRect(HWND hWnd, RECT* lpRect);

    [DebuggerStepThrough]
    public static unsafe BOOL GetWindowRect(HWND hWnd, out RECT rect)
    {
        fixed (RECT* lpRect = &rect)
        {
            return GetWindowRect(hWnd, lpRect);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe uint GetWindowThreadProcessId(HWND hWnd, [MaybeNull] uint* lpdwProcessId);

    [DebuggerStepThrough]
    public static unsafe uint GetWindowThreadProcessId(HWND hWnd, [MaybeNull] out uint dwProcessId)
    {
        fixed (uint* lpdwProcessId = &dwProcessId)
        {
            return GetWindowThreadProcessId(hWnd, lpdwProcessId);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe BOOL IntersectRect([Out] RECT* lprcDst, RECT* lprcSrc1, RECT* lprcSrc2);

    public static unsafe BOOL IntersectRect(out RECT rcDst, ref readonly RECT rcSrc1, ref readonly RECT rcSrc2)
    {
        fixed (RECT* lprcDst = &rcDst)
        {
            fixed (RECT* lprcSrc1 = &rcSrc1)
            {
                fixed (RECT* lprcSrc2 = &rcSrc2)
                {
                    return IntersectRect(lprcDst, lprcSrc1, lprcSrc2);
                }
            }
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL IsIconic(HWND hWnd);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL IsWindowVisible(HWND hWnd);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL PostThreadMessageW(uint idThread, uint Msg, WPARAM wParam, LPARAM lParam);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe ushort RegisterClassW(WNDCLASSW* lpWndClass);

    public static unsafe ushort RegisterClassW(ref readonly WNDCLASSW lpWndClass)
    {
        fixed (WNDCLASSW* pWndClass = &lpWndClass)
        {
            return RegisterClassW(pWndClass);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows6.0.6000")]
    public static extern BOOL RegisterHotKey([Optional] HWND hWnd, int id, HOT_KEY_MODIFIERS fsModifiers, uint vk);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern uint RegisterWindowMessageW(PCWSTR lpString);

    public static unsafe uint RegisterWindowMessageW(string @string)
    {
        fixed (char* lpString = @string)
        {
            return RegisterWindowMessageW(lpString);
        }
    }

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern int ReleaseDC([Optional] HWND hWnd, HDC hDC);

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
    public static extern BOOL SetLayeredWindowAttributes(HWND hwnd, COLORREF crKey, byte bAlpha, LAYERED_WINDOW_ATTRIBUTES_FLAGS dwFlags);

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
    public static extern BOOL ShowWindow(HWND hWnd, SHOW_WINDOW_CMD nCmdShow);

    [DllImport("USER32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern BOOL UnregisterHotKey([Optional] HWND hWnd, int id);
}