// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Snap.Hutao.Core.Windowing.Backdrop;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using Windows.Foundation;
using WinRT.Interop;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Core.Windowing.NotifyIcon;

internal sealed class NotifyIconXamlHostWindow : Window, IDisposable, IWindowNeedEraseBackground
{
    private readonly XamlWindowSubclass subclass;

    public NotifyIconXamlHostWindow()
    {
        Content = new Border();

        this.SetLayeredWindow();

        AppWindow.Title = "SnapHutaoNotifyIconXamlHost";
        AppWindow.IsShownInSwitchers = false;
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsResizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.SetBorderAndTitleBar(false, false);
        }

        XamlWindowOptions options = new(this, default!, default);
        subclass = new(this, options);
        subclass.Initialize();

        Activate();
    }

    public void ShowFlyoutAt(FlyoutBase flyout, Point point, RECT icon)
    {
        icon.left -= 8;
        icon.top -= 8;
        icon.right += 8;
        icon.bottom += 8;

        HWND hwnd = WindowNative.GetWindowHandle(this);
        ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_NORMAL);
        SetForegroundWindow(hwnd);

        AppWindow.MoveAndResize(StructMarshal.RectInt32(icon));
        flyout.ShowAt(Content, new()
        {
            Placement = FlyoutPlacementMode.Auto,
            ShowMode = FlyoutShowMode.Transient,
        });
    }

    public void Dispose()
    {
        subclass.Dispose();
    }
}