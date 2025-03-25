// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Input;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.ViewModel.Overlay;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Singleton)]
internal sealed partial class LaunchExecutionOverlayWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowMouseWheelHandler,
    IWindowNeedEraseBackground
{
    public LaunchExecutionOverlayWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        AppWindow.Title = "SnapHutaoLaunchExecutionOverlay";
        AppWindow.IsShownInSwitchers = false;

        SizeInt32 size = ScaledSizeInt32.CreateForWindow(386, 56, this);

        // Thanks to @Scighost for the following code
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.SetBorderAndTitleBar(false, false);
            presenter.PreferredMinimumWidth = size.Width;
            presenter.PreferredMinimumHeight = size.Height;
            presenter.PreferredMaximumWidth = size.Height;
            presenter.PreferredMaximumHeight = size.Height;
        }

        this.AddExStyleLayered();
        this.RemoveStyleOverlappedWindow();

        SystemBackdrop = new TransparentBackdrop();

        this.InitializeController(serviceProvider);
        RootView.InitializeDataContext<OverlayViewModel>(serviceProvider);
        AppWindow.Resize(size);
    }

    public FrameworkElement TitleBarCaptionAccess { get => RootBorder; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => []; }

    public void OnMouseWheel(ref readonly PointerPointProperties data)
    {
        RootView.DataContext<OverlayViewModel>()?.HandleMouseWheel(data.Delta / 120);
    }
}