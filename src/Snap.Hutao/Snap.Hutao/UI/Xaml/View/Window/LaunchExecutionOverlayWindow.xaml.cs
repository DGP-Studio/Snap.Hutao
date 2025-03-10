// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml.Media.Backdrop;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Singleton)]
internal sealed partial class LaunchExecutionOverlayWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IWindowNeedEraseBackground
{
    public LaunchExecutionOverlayWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        AppWindow.Title = "SnapHutaoLaunchExecutionOverlay";
        AppWindow.IsShownInSwitchers = false;

        OverlappedPresenter presenter = OverlappedPresenter.CreateForContextMenu();
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        presenter.IsResizable = false;
        presenter.IsAlwaysOnTop = true;
        presenter.SetBorderAndTitleBar(true, false);
        AppWindow.SetPresenter(presenter);

        AppWindow.Resize(ScaledSizeInt32.CreateForWindow(320, 56, this));

        this.AddExStyleLayered();

        SystemBackdrop = new TransparentBackdrop();

        this.InitializeController(serviceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => RootGrid; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => []; }
}