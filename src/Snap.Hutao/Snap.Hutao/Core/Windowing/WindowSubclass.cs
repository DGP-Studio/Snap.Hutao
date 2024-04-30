// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing.Backdrop;
using Snap.Hutao.Core.Windowing.HotKey;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using static Snap.Hutao.Win32.ComCtl32;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 窗体子类管理器
/// </summary>
[HighQuality]
internal sealed class WindowSubclass : IDisposable
{
    private const int WindowSubclassId = 101;

    private readonly Window window;
    private readonly WindowOptions options;
    private readonly IServiceProvider serviceProvider;
    private readonly IHotKeyController hotKeyController;

    // We have to explicitly hold a reference to SUBCLASSPROC
    private SUBCLASSPROC windowProc = default!;

    public WindowSubclass(Window window, in WindowOptions options, IServiceProvider serviceProvider)
    {
        this.window = window;
        this.options = options;
        this.serviceProvider = serviceProvider;

        hotKeyController = serviceProvider.GetRequiredService<IHotKeyController>();
    }

    /// <summary>
    /// 尝试设置窗体子类
    /// </summary>
    /// <returns>是否设置成功</returns>
    public bool Initialize()
    {
        windowProc = OnSubclassProcedure;
        bool windowHooked = SetWindowSubclass(options.Hwnd, windowProc, WindowSubclassId, 0);
        hotKeyController.RegisterAll();

        return windowHooked;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        hotKeyController.UnregisterAll();

        RemoveWindowSubclass(options.Hwnd, windowProc, WindowSubclassId);
        windowProc = default!;
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
                        handler.HandleMinMaxInfo(ref *(MINMAXINFO*)lParam, options.GetRasterizationScale());
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

            case WM_ERASEBKGND:
                {
                    if (window.SystemBackdrop is IBackdropNeedEraseBackground)
                    {
                        return (LRESULT)(int)BOOL.TRUE;
                    }

                    break;
                }
        }

        return DefSubclassProc(hwnd, uMsg, wParam, lParam);
    }
}