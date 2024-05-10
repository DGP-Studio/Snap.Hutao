// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing.Backdrop;
using Snap.Hutao.Core.Windowing.HotKey;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ComCtl32;
using static Snap.Hutao.Win32.ConstValues;

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
    private UnmanagedAccess<WindowSubclass> unmanagedAccess = default!;

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
    public unsafe bool Initialize()
    {
        windowProc = SUBCLASSPROC.Create(&OnSubclassProcedure);
        unmanagedAccess = UnmanagedAccess.Create(this);
        bool windowHooked = SetWindowSubclass(options.Hwnd, windowProc, WindowSubclassId, unmanagedAccess);
        hotKeyController.RegisterAll();

        return windowHooked;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        hotKeyController.UnregisterAll();

        RemoveWindowSubclass(options.Hwnd, windowProc, WindowSubclassId);
        windowProc = default!;
        unmanagedAccess.Dispose();
    }

    [SuppressMessage("", "SH002")]
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe LRESULT OnSubclassProcedure(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        WindowSubclass? state = UnmanagedAccess.Get<WindowSubclass>(dwRefData);
        ArgumentNullException.ThrowIfNull(state);

        switch (uMsg)
        {
            case WM_GETMINMAXINFO:
                {
                    if (state.window is IMinMaxInfoHandler handler)
                    {
                        handler.HandleMinMaxInfo(ref *(MINMAXINFO*)lParam, state.options.GetRasterizationScale());
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
                    state.hotKeyController.OnHotKeyPressed(*(HotKeyParameter*)&lParam);
                    break;
                }

            case WM_ERASEBKGND:
                {
                    if (state.window.SystemBackdrop is IBackdropNeedEraseBackground)
                    {
                        return (LRESULT)(int)BOOL.TRUE;
                    }

                    break;
                }
        }

        return DefSubclassProc(hwnd, uMsg, wParam, lParam);
    }
}