// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core.Windowing.HotKey;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 窗体子类管理器
/// </summary>
[HighQuality]
internal sealed class WindowSubclass : IDisposable
{
    private const int WindowSubclassId = 101;
    private const int DragBarSubclassId = 102;

    private readonly Window window;
    private readonly WindowOptions options;
    private readonly IServiceProvider serviceProvider;
    private readonly IHotKeyController hotKeyController;

    // We have to explicitly hold a reference to SUBCLASSPROC
    private SUBCLASSPROC? windowProc;
    private SUBCLASSPROC? legacyDragBarProc;

    public WindowSubclass(Window window, in WindowOptions options, IServiceProvider serviceProvider)
    {
        this.window = window;
        this.options = options;
        this.serviceProvider = serviceProvider;
        hotKeyController = new HotKeyController(serviceProvider);
    }

    /// <summary>
    /// 尝试设置窗体子类
    /// </summary>
    /// <returns>是否设置成功</returns>
    public bool Initialize()
    {
        windowProc = OnSubclassProcedure;
        bool windowHooked = SetWindowSubclass(options.Hwnd, windowProc, WindowSubclassId, 0);
        hotKeyController.Register(options.Hwnd);

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
        hotKeyController.Unregister(options.Hwnd);

        RemoveWindowSubclass(options.Hwnd, windowProc, WindowSubclassId);
        windowProc = null;

        if (options.UseLegacyDragBarImplementation)
        {
            RemoveWindowSubclass(options.Hwnd, legacyDragBarProc, DragBarSubclassId);
            legacyDragBarProc = null;
        }
    }

    [SuppressMessage("", "SH002")]
    private unsafe LRESULT OnSubclassProcedure(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        switch (uMsg)
        {
            case WM_GETMINMAXINFO:
                {
                    if (window is IMinMaxInfoHandler handler)
                    {
                        handler.HandleMinMaxInfo(ref *(MINMAXINFO*)lParam.Value, options.GetWindowScale());
                    }

                    break;
                }

            case WM_NCRBUTTONDOWN:
            case WM_NCRBUTTONUP:
                {
                    return default;
                }

            case WM_HOTKEY:
                {
                    hotKeyController.OnHotKeyPressed(*(HotKeyParameter*)&lParam);
                    break;
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