// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.ViewModel.Overlay;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Singleton)]
internal sealed partial class LaunchExecutionOverlayWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IWindowNeedEraseBackground
{
    public LaunchExecutionOverlayWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        RootView.InitializeDataContext<OverlayViewModel>(serviceProvider);

        AppWindow.Title = "SnapHutaoLaunchExecutionOverlay";
        AppWindow.IsShownInSwitchers = false;

        // Thanks to @Scighost for the following code
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.SetBorderAndTitleBar(false, false);
        }

        AppWindow.Resize(ScaledSizeInt32.CreateForWindow(320, 56, this));

        this.AddExStyleLayered();
        this.RemoveStyleOverlappedWindow();

        SystemBackdrop = new TransparentBackdrop();

        this.InitializeController(serviceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => RootBorder; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => []; }
}