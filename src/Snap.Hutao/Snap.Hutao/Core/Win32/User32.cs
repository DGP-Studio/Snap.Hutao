// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.Win32;

/// <summary>
/// 包含 user32.dll 平台调用的代码
/// </summary>
internal static class User32
{
    [SuppressMessage("", "SA1600")]
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    /// <summary>
    /// Sets the show state and the restored, minimized, and maximized positions of the specified window.
    /// </summary>
    /// <param name="hWnd">
    /// A handle to the window.
    /// </param>
    /// <param name="lpwndpl">
    /// A pointer to a WINDOWPLACEMENT structure that specifies the new show state and window positions.
    /// <para>
    /// Before calling SetWindowPlacement, set the length member of the WINDOWPLACEMENT structure to sizeof(WINDOWPLACEMENT). SetWindowPlacement fails if the length member is not set correctly.
    /// </para>
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero.
    /// <para>
    /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
    /// </para>
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

    /// <summary>
    /// Changes the text of the specified window's title bar (if it has one). If the specified window is a control, the
    /// text of the control is changed. However, SetWindowText cannot change the text of a control in another application.
    /// <para>
    /// Go to <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633546%28v=vs.85%29.aspx"/> for more
    /// information
    /// </para>
    /// </summary>
    /// <param name="hwnd">C++ ( hWnd [in]. Type: HWND )<br />A handle to the window or control whose text is to be changed.</param>
    /// <param name="lpString">C++ ( lpString [in, optional]. Type: LPCTSTR )<br />The new title or control text.</param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.<br />
    /// To get extended error information, call GetLastError.
    /// </returns>
    /// <remarks>
    /// If the target window is owned by the current process, <see cref="SetWindowText" /> causes a WM_SETTEXT message to
    /// be sent to the specified window or control. If the control is a list box control created with the WS_CAPTION style,
    /// however, <see cref="SetWindowText" /> sets the text for the control, not for the list box entries.<br />To set the
    /// text of a control in another process, send the WM_SETTEXT message directly instead of calling
    /// <see cref="SetWindowText" />. The <see cref="SetWindowText" /> function does not expand tab characters (ASCII code
    /// 0x09). Tab characters are displayed as vertical bar(|) characters.<br />For an example go to
    /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms644928%28v=vs.85%29.aspx#sending">Sending a Message. </see>
    /// </remarks>
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool SetWindowText(IntPtr hwnd, string lpString);
}
