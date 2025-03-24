// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.ViewModel.Guide;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Singleton)]
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
            SizeInt32 maxSize = ScaledSizeInt32.CreateForWindow(1200, 800, this);
            presenter.PreferredMinimumWidth = minSize.Width;
            presenter.PreferredMinimumHeight = minSize.Height;
            presenter.PreferredMaximumWidth = maxSize.Width;
            presenter.PreferredMaximumHeight = maxSize.Height;
        }

        IServiceScope scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
        GuideView.InitializeDataContext<GuideViewModel>(scope.ServiceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => DraggableGrid; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => []; }

    public SizeInt32 InitSize { get => ScaledSizeInt32.CreateForWindow(1000, 650, this); }
}