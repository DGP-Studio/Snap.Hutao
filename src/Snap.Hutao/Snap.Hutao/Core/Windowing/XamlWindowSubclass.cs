// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing.Abstraction;
using Snap.Hutao.Core.Windowing.Backdrop;
using Snap.Hutao.Core.Windowing.NotifyIcon;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ComCtl32;
using static Snap.Hutao.Win32.ConstValues;

namespace Snap.Hutao.Core.Windowing;

[HighQuality]
internal sealed class XamlWindowSubclass : IDisposable
{
    private const int WindowSubclassId = 101;

    private readonly Window window;
    private readonly HWND hwnd;

    // We have to explicitly hold a reference to SUBCLASSPROC
    private SUBCLASSPROC windowProc = default!;
    private UnmanagedAccess<XamlWindowSubclass> unmanagedAccess = default!;

    public XamlWindowSubclass(Window window)
    {
        this.window = window;
        hwnd = window.GetWindowHandle();
    }

    public unsafe bool Initialize()
    {
        windowProc = SUBCLASSPROC.Create(&OnSubclassProcedure);
        unmanagedAccess = UnmanagedAccess.Create(this);
        return SetWindowSubclass(hwnd, windowProc, WindowSubclassId, unmanagedAccess);
    }

    public void Dispose()
    {
        RemoveWindowSubclass(hwnd, windowProc, WindowSubclassId);
        windowProc = default!;
        unmanagedAccess.Dispose();
    }

    [SuppressMessage("", "SH002")]
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe LRESULT OnSubclassProcedure(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        XamlWindowSubclass? state = UnmanagedAccess.Get<XamlWindowSubclass>(dwRefData);
        ArgumentNullException.ThrowIfNull(state);

        switch (uMsg)
        {
            case WM_GETMINMAXINFO:
                {
                    if (state.window is IXamlWindowSubclassMinMaxInfoHandler handler)
                    {
                        handler.HandleMinMaxInfo(ref *(MINMAXINFO*)lParam, state.window.GetRasterizationScale());
                    }

                    break;
                }

            case WM_NCRBUTTONDOWN:
            case WM_NCRBUTTONUP:
                {
                    return default;
                }

            case WM_NCLBUTTONDBLCLK:
                {
                    if (state.window.AppWindow.Presenter is OverlappedPresenter { IsMaximizable: false })
                    {
                        return default;
                    }

                    break;
                }

            case WM_ERASEBKGND:
                {
                    if (state.window is IWindowNeedEraseBackground || state.window.SystemBackdrop is IBackdropNeedEraseBackground)
                    {
                        return (LRESULT)(int)BOOL.TRUE;
                    }

                    break;
                }
        }

        return DefSubclassProc(hwnd, uMsg, wParam, lParam);
    }
}