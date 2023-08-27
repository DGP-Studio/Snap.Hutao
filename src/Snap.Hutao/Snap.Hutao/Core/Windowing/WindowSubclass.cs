// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 窗体子类管理器
/// </summary>
/// <typeparam name="TWindow">窗体类型</typeparam>
[HighQuality]
internal sealed class WindowSubclass<TWindow> : IDisposable
    where TWindow : Window, IWindowOptionsSource
{
    private const int WindowSubclassId = 101;
    private const int DragBarSubclassId = 102;

    private readonly TWindow window;

    // We have to explicitly hold a reference to SUBCLASSPROC
    private SUBCLASSPROC? windowProc;
    private SUBCLASSPROC? legacyDragBarProc;

    /// <summary>
    /// 构造一个新的窗体子类管理器
    /// </summary>
    /// <param name="window">窗口</param>
    public WindowSubclass(TWindow window)
    {
        this.window = window;
    }

    /// <summary>
    /// 尝试设置窗体子类
    /// </summary>
    /// <returns>是否设置成功</returns>
    public bool Initialize()
    {
        WindowOptions options = window.WindowOptions;

        windowProc = OnSubclassProcedure;
        bool windowHooked = SetWindowSubclass(options.Hwnd, windowProc, WindowSubclassId, 0);

        bool titleBarHooked = true;

        // only hook up drag bar proc when use legacy Window.ExtendsContentIntoTitleBar
        if (!options.UseLegacyDragBarImplementation)
        {
            return windowHooked && titleBarHooked;
        }

        titleBarHooked = false;
        HWND hwndDragBar = FindWindowEx(options.Hwnd, default, "DRAG_BAR_WINDOW_CLASS", default);

        if (hwndDragBar.IsNull)
        {
            return windowHooked && titleBarHooked;
        }

        legacyDragBarProc = OnLegacyDragBarProcedure;
        titleBarHooked = SetWindowSubclass(hwndDragBar, legacyDragBarProc, DragBarSubclassId, 0);

        return windowHooked && titleBarHooked;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        WindowOptions options = window.WindowOptions;

        RemoveWindowSubclass(options.Hwnd, windowProc, WindowSubclassId);
        windowProc = null;

        if (!options.UseLegacyDragBarImplementation)
        {
            return;
        }

        RemoveWindowSubclass(options.Hwnd, legacyDragBarProc, DragBarSubclassId);
        legacyDragBarProc = null;
    }

    [SuppressMessage("", "SH002")]
    private unsafe LRESULT OnSubclassProcedure(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        switch (uMsg)
        {
            case WM_GETMINMAXINFO:
                {
                    uint dpi = GetDpiForWindow(hwnd);
                    double scalingFactor = Math.Round(dpi / 96D, 2, MidpointRounding.AwayFromZero);
                    window.ProcessMinMaxInfo((MINMAXINFO*)lParam.Value, scalingFactor);
                    break;
                }

            case WM_NCRBUTTONDOWN:
            case WM_NCRBUTTONUP:
                {
                    return (LRESULT)(nint)WM_NULL;
                }
        }

        return DefSubclassProc(hwnd, uMsg, wParam, lParam);
    }

    [SuppressMessage("", "SH002")]
    private LRESULT OnLegacyDragBarProcedure(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        switch (uMsg)
        {
            case WM_NCRBUTTONDOWN:
            case WM_NCRBUTTONUP:
                {
                    return (LRESULT)(nint)WM_NULL;
                }
        }

        return DefSubclassProc(hwnd, uMsg, wParam, lParam);
    }
}