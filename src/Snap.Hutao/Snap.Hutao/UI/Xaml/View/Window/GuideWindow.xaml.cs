// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.ViewModel.Guide;
using System.Collections.Immutable;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class GuideWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowHasInitSize
{
    public GuideWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            SizeInt32 minSize = ScaledSizeInt32.CreateForWindow(1000, 650, this);
            presenter.PreferredMinimumWidth = minSize.Width;
            presenter.PreferredMinimumHeight = minSize.Height;
            SizeInt32 maxSize = ScaledSizeInt32.CreateForWindow(1200, 800, this);
            presenter.PreferredMaximumWidth = maxSize.Width;
            presenter.PreferredMaximumHeight = maxSize.Height;
        }

        IServiceScope scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
        GuideView.InitializeDataContext<GuideViewModel>(scope.ServiceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => DraggableGrid; }

    public ImmutableArray<FrameworkElement> TitleBarPassthrough { get => []; }

    public SizeInt32 InitSize { get => ScaledSizeInt32.CreateForWindow(1000, 650, this); }
}