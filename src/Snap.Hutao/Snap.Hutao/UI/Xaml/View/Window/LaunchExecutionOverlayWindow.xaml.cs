// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dwm;
using System.Runtime.CompilerServices;
using static Snap.Hutao.Win32.DwmApi;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Singleton)]
internal sealed partial class LaunchExecutionOverlayWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IWindowNeedEraseBackground
{
    public LaunchExecutionOverlayWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        this.AddExStyleLayered();
        this.AddExStyleToolWindow();

        AppWindow.Title = "SnapHutaoLaunchExecutionOverlay";
        AppWindow.IsShownInSwitchers = false;
        OverlappedPresenter presenter = OverlappedPresenter.CreateForContextMenu();
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        presenter.IsResizable = false;
        presenter.IsAlwaysOnTop = true;
        presenter.SetBorderAndTitleBar(false, false);
        AppWindow.SetPresenter(presenter);
        AppWindow.Resize(ScaledSizeInt32.CreateForWindow(320, 56, this));

        SystemBackdrop = new TransparentBackdrop();

        this.InitializeController(serviceProvider);

        uint color = 0xFFFFFFFE;
        DwmSetWindowAttribute(this.GetWindowHandle(), DWMWINDOWATTRIBUTE.DWMWA_BORDER_COLOR, ref Unsafe.As<uint, COLORREF>(ref color));
    }

    public FrameworkElement TitleBarCaptionAccess { get => RootGrid; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => []; }
}