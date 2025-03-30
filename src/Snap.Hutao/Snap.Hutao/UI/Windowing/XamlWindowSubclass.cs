// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Input;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.SystemServices;
using Snap.Hutao.Win32.UI.Shell;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ComCtl32;
using static Snap.Hutao.Win32.ConstValues;

namespace Snap.Hutao.UI.Windowing;

internal sealed partial class XamlWindowSubclass : IDisposable
{
    private const int WindowSubclassId = 101;

    private readonly Window window;

    // We have to explicitly hold a reference to SUBCLASSPROC
    private SUBCLASSPROC windowProc;
    private GCHandle unmanagedAccess;

    public XamlWindowSubclass(Window window)
    {
        this.window = window;
    }

    public unsafe bool Initialize()
    {
        windowProc = SUBCLASSPROC.Create(&OnSubclassProcedure);
        unmanagedAccess = GCHandle.Alloc(this);
        return SetWindowSubclass(window.GetWindowHandle(), windowProc, WindowSubclassId, (nuint)GCHandle.ToIntPtr(unmanagedAccess));
    }

    public void Dispose()
    {
        RemoveWindowSubclass(window.GetWindowHandle(), windowProc, WindowSubclassId);
        windowProc = default!;
        unmanagedAccess.Free();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe LRESULT OnSubclassProcedure(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        if (XamlApplicationLifetime.Exiting)
        {
            return default;
        }

        XamlWindowSubclass? state = GCHandle.FromIntPtr((nint)dwRefData).Target as XamlWindowSubclass;
        ArgumentNullException.ThrowIfNull(state);

        switch (uMsg)
        {
            case WM_PAINT:
                {
                    DwmApi.DwmFlush();
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
                    if (state.window is IWindowNeedEraseBackground ||
                        state.window.SystemBackdrop is IBackdropNeedEraseBackground)
                    {
                        return (int)BOOL.TRUE;
                    }

                    break;
                }

            case WM_MOUSEWHEEL:
                {
                    if (state.window is IXamlWindowMouseWheelHandler handler)
                    {
                        WPARAM2MOUSEWHEEL pWParam2 = *(WPARAM2MOUSEWHEEL*)&wParam;
                        LPARAM2MOUSEWHEEL pLParam2 = *(LPARAM2MOUSEWHEEL*)&lParam;
                        PointerPointProperties data = new(pWParam2.High, (MODIFIERKEYS_FLAGS)pWParam2.Low, pLParam2.Low, pLParam2.High);
                        handler.OnMouseWheel(in data);
                    }

                    break;
                }
        }

        return DefSubclassProc(hwnd, uMsg, wParam, lParam);
    }

#pragma warning disable CA1823
#pragma warning disable CS0169
#pragma warning disable CS0649

    // ReSharper disable once InconsistentNaming
    private readonly struct WPARAM2MOUSEWHEEL
    {
        public readonly short Low;
        public readonly short High;
        private readonly int reserved;
    }

    // ReSharper disable once InconsistentNaming
    private readonly struct LPARAM2MOUSEWHEEL
    {
        public readonly short Low;
        public readonly short High;
        private readonly int reserved;
    }

#pragma warning restore CS0649
#pragma warning restore CS0169
#pragma warning restore CA1823
}